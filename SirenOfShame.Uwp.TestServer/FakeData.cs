using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SirenOfShame.Uwp.Watcher.Watcher;

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

        public static string GetFakeBuilds()
        {
            var buildStatuses = new RefreshStatusEventArgs()
            {
                BuildStatusDtos = new List<BuildStatusDto>
                {
                    MakeBuildDefinition(1, "Build Def #1", BuildStatusEnum.InProgress),
                    MakeBuildDefinition(2, "Build Def #2", BuildStatusEnum.Broken),
                    MakeBuildDefinition(3, "Build Def #3", BuildStatusEnum.Unknown),
                    MakeBuildDefinition(4, "Build Def #4"),
                    MakeBuildDefinition(5, "Build Def #5"),
                    MakeBuildDefinition(6, "Build Def #6"),
                }
            };
            return JsonConvert.SerializeObject(buildStatuses);
        }

        private static BuildStatusDto MakeBuildDefinition(int id, string title, BuildStatusEnum buildStatusEnum = BuildStatusEnum.Working)
        {
            return new BuildStatusDto
            {
                BuildDefinitionId = id.ToString(),
                BuildDefinitionDisplayName = title,
                BuildStatusEnum = buildStatusEnum,
                RequestedByDisplayName = "Lee Richardson",
                LocalStartTime = DateTime.Now,
                Comment = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                Duration = "1:15"
            };
        }

    }
}
