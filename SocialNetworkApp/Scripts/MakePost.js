
function initPostButtons() {
    $(function () {
        $("#postcontainer").draggable(
        {
        handle: 'h1'
        }
        );
    });

    $("#Tlocation_Latitude").val(userLat);
    $("#Tlocation_Longitude").val(userLong);

    $("#closepostbtn").onsubmit 
    $("#closepostbtn").click(function () {
        $("#postcontainer").hide();
        removeRadiusSelctor();
    });

}