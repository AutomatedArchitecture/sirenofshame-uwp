namespace SirenOfShame.Uwp.TestServer
{
    public static class FakeData
    {
        public static string NewNewsItem { get; } = @"{
    ""BuildDefinitionId"": null,
    ""EventDate"": ""2016-12-17T09:45:17.2015184-05:00"",
    ""Person"": {
        ""RawName"": ""Bob Shimpty"",
        ""DisplayName"": ""Bob Shimpty"",
        ""TotalBuilds"": 200,
        ""FailedBuilds"": 20,
        ""Hidden"": false,
        ""Achievements"": [],
        ""CumulativeBuildTime"": null,
        ""AvatarId"": null,
        ""NumberOfTimesFixedSomeoneElsesBuild"": 1,
        ""NumberOfTimesPerformedBackToBackBuilds"": 0,
        ""MaxBuildsInOneDay"": 0,
        ""CurrentBuildRatio"": 2.0,
        ""LowestBuildRatioAfter50Builds"": null,
        ""CurrentSuccessInARow"": 2,
        ""Email"": null,
        ""AvatarImageName"": null,
        ""AvatarImageUploaded"": false,
        ""Clickable"": true
    },
    ""Title"": ""Achieved Shame Pusher"",
    ""AvatarImageList"": null,
    ""NewsItemType"": 0,
    ""ReputationChange"": 1,
    ""BuildId"": null,
    ""IsSosOnlineEvent"": true,
    ""ShouldUpdateOldInProgressNewsItem"": false
}";

        public static string NewUserItem { get; } = @"{
        ""RawName"": ""Bob Shimpty"",
        ""DisplayName"": ""Bob Shimpty"",
        ""TotalBuilds"": 200,
        ""FailedBuilds"": 20,
        ""Hidden"": false,
        ""Achievements"": [],
        ""CumulativeBuildTime"": null,
        ""AvatarId"": null,
        ""NumberOfTimesFixedSomeoneElsesBuild"": 1,
        ""NumberOfTimesPerformedBackToBackBuilds"": 0,
        ""MaxBuildsInOneDay"": 0,
        ""CurrentBuildRatio"": 2.0,
        ""LowestBuildRatioAfter50Builds"": null,
        ""CurrentSuccessInARow"": 2,
        ""Email"": null,
        ""AvatarImageName"": null,
        ""AvatarImageUploaded"": false,
        ""Clickable"": true
    }";
    }
}
