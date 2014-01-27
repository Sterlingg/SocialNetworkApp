
SELECT 
[Extent1].[UserID] AS [UserID], 
[Extent2].[Latitude] AS [Latitude], 
[Extent2].[Longitude] AS [Longitude], 
[Join2].[PostContent] AS [PostContent], 
[Join2].[PostTitle] AS [PostTitle]
FROM   [dbo].[Subscription] AS [Extent1]
CROSS JOIN [dbo].[Location] AS [Extent2]
INNER JOIN  (SELECT [Extent3].[PostTitle] AS [PostTitle], [Extent3].[PostContent] AS [PostContent], [Extent3].[VisibleProximity] AS [VisibleProximity], [Extent3].[GroupID] AS [GroupID], [Extent3].[LocationID] AS [LocationID1], [Extent4].[LocationID] AS [LocationID2], [Extent4].[Longitude] AS [Longitude], [Extent4].[Latitude] AS [Latitude]
 FROM  [dbo].[Post] AS [Extent3]
 CROSS JOIN [dbo].[Location] AS [Extent4] ) AS [Join2] ON ([Extent1].[UserID] = [Join2].[GroupID]) AND ([Extent2].[LocationID] = [Join2].[LocationID1])
WHERE ([Extent1].[GroupID] = 1) AND ((([Join2].[VisibleProximity] IS NOT NULL) AND (((([Join2].[Latitude] - -123.114192900000000) * ([Join2].[Latitude] - -123.114192900000000) * 111212.22 * 111212.22) + (([Join2].[Longitude] - 49.234178200000000) * ([Join2].[Longitude] - 49.234178200000000) * 111212.22 * 111212.22)) < ([Join2].[VisibleProximity] * [Join2].[VisibleProximity])) AND ([Join2].[LocationID1] = [Join2].[LocationID2])) OR ([Join2].[VisibleProximity] IS NULL))
