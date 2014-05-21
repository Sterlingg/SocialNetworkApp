var map;

var infowindow;

var isGeoDone = false;
var post_array = [];
var user_location;
var userLat;
var userLong;
var radius_selector = null;
var oms;

//Container object for a post.

function Post(title, content, lat, lon, marker, gid, isShown) {

    this.title = title;
    this.lat = lat;
    this.lon = lon;
    this.content = content;
    this.marker = marker;
    this.gid = gid;
    this.isShown = isShown;
}

// Add a marker to the map, and store the posts contents to the post_array.
// With spiderfication for overlapping posts.
// Courtesy of https://github.com/jawj/OverlappingMarkerSpiderfier


// Adds a marker to the map without saving it anywhere.
function addMarker(lat, lon) {
    var pos = new google.maps.LatLng(lat, lon);
    var marker = new google.maps.Marker({
        position: pos,
        map: map,
    });
    oms.addMarker(marker);
}

function addPost(title, content, lat, lon, gid) {

    var pos = new google.maps.LatLng(lat, lon);
    var marker = new google.maps.Marker({
        title: getContentString(title, content),
        position: pos,
        map: map,
    });
   
    marker.iwcontent = getContentString(title, content);

    oms.addMarker(marker);
    map.panTo(marker.getPosition());
    var aPost = new Post(title, content, lat, lon, marker, gid, true);

    post_array.push(aPost);
}

// doGeoLocation: Called after map is loaded to move the map to the users current location, and
//set the users location preference.
function doGeoAndPosts() {
    navigator.geolocation.getCurrentPosition(
    //Success
    function (position) {
        var coordinates = position.coords;
        var latitude = coordinates.latitude;
        userLat = latitude;
        var longitude = coordinates.longitude;
        userLong = longitude;

        $("#lat").val(latitude);
        $("#long").val(longitude);
        map.panTo(new google.maps.LatLng(latitude, longitude));
        map.setZoom(10);
        isGeoDone = true;
        initMarkers();
    }
    ,
    //Using default values.
    function (position) {

        $("#lat").val(userLat);
        $("#long").val(userLong);
        map.panTo(new google.maps.LatLng(userLat, userLong));
        map.setZoom(10);
        isGeoDone = true;
        initMarkers();
    }
    );
}

// Add a circle overlay to the map that allows the user to select where their
// post is viewable. 
function drawCircleOverlay(lat, lon) {
    var ignore = false;
    var center_point = new google.maps.LatLng(lat, lon);
    var circleOptions = {
        center: center_point,
        radius: 3000,
        fillOpacity: 0.1,       
        map: map,
        editable: true
    };
    if (radius_selector == null) {
        radius_selector = new google.maps.Circle(circleOptions);

        google.maps.event.addListener(radius_selector, 'radius_changed', function () {
            radius = radius_selector.getRadius();
            $("#vpinput").val(radius);

            if (radius < 2000) {
                radius_selector.setOptions({
                    strokeColor: "FF0313",
                    fillColor: "FF0313"
                });
            }
            else if (radius > 2000 && radius < 6000) {
                radius_selector.setOptions({
                    strokeColor: "E08916",
                    fillColor: "E08916"
                });
            }
            else {
                radius_selector.setOptions({
                    strokeColor: "3EFF29",
                    fillColor: "3EFF29"
                });

            }


        });

        //http://stackoverflow.com/questions/11898108/google-maps-v3-setting-a-circle-with-editable-radius-but-not-center
        // Disallows the circle overlay to be moved, instead only allowing changing the
        // radius.
        google.maps.event.addListener(radius_selector, 'center_changed', function () {
            if (ignore) {
                ignore = false;
                return;
            }
            radius_selector.setEditable(false);
            ignore = true;
            radius_selector.setCenter(center_point);
            radius_selector.setEditable(true);

        });
    }
    else
        radius_selector.setMap(map);
}

// Returns a string of html for a given posts content window.
function getContentString(title, content) {
    var contentString = '<div id="content">' +
    '<div id="siteNotice">' +
    '</div>' +
    '<h2 id="firstHeading" class="firstHeading">' + title + '</h2>' +
    '<div id="bodyContent">' +
    '<p>' + content + '</p>' +
    '</div>' +
    '</div>';

    return contentString;
}

//initialize: Set up the initial map, and do the geo location and show the posts on the map.
function initializeMap() {

    userLat = -123.114192900000000;
    userLong = 49.234178200000000;

    infowindow = new google.maps.InfoWindow();
    var options = { center: new google.maps.LatLng(-123.114192900000000, 49.234178200000000),
        zoom: 8,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    map = new google.maps.Map(document.getElementById("theMap"), options);

    oms = new OverlappingMarkerSpiderfier(map);

    // Each marker has a content string for their info box.
    oms.addListener('click', function (marker) {
        infowindow.setContent(marker.iwcontent);
        infowindow.open(map, marker);
    });

    oms.addListener('spiderfy', function (markers) {
        infowindow.close();
    });
    if (isGeoDone == false) {
        doGeoAndPosts();
    }
    else {
        map.panTo(new google.maps.LatLng(userLat, userLong));
        map.setZoom(10);
        initMarkers();
    }

}

// Query the database to retrieve the posts, and display them on the map.
// TODO: Add filtering for groups.
function initMarkers() {
    var url = '/Map/GetPosts';
    $.getJSON(url, { lat: userLat + '0', lon: userLong + '0' }, function (data) {
        $.each(data, function (key, val) {
            addPost(val.Title, val.Content, val.Latitude, val.Longitude, val.GroupID);
        });
    });
}

// Remove the circle overlay from the map after the post is made.
function removeRadiusSelctor() {
    radius_selector.setMap(null);
}

// When a "show" check box is checked / unchecked update the map
// to reflect this change.
function updateMapFromCheck(checkID) {
    var checkstr = "#c" + checkID;

    var isChecked = $(checkstr).is(':checked');

    for (var i = 0; i < post_array.length; i++) {
        if ((post_array[i]).gid == checkID) {
            if (isChecked == true) {
                if ((post_array[i]).isShown == false) {
                    (post_array[i]).marker.setMap(map);
                    (post_array[i]).isShown = true;
                    oms.addMarker((post_array[i]).marker);
                }
            }
            else {
                (post_array[i]).marker.setMap(null);
                (post_array[i]).isShown = false;
                oms.removeMarker((post_array[i]).marker);
            }
        }
    }

}

// Query the database to retrieve the posts, and display them on the map clearing the
// previous ones first.
function updateMarkers() {
    var url = '/Map/GetPosts';

    for (var i = 0; i < post_array.length; i++) {
        (post_array[i]).marker.setMap(null);
        oms.removeMarker((post_array[i]).marker);
    }
    post_array = [];

    $.getJSON(url, { lat: userLat + '0', lon: userLong + '0' }, function (data) {
        $.each(data, function (key, val) {
            addPost(val.Title, val.Content, val.Latitude, val.Longitude, val.GroupID);
        });
    });
}
