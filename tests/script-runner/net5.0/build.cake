#addin nuget:?package=Cake.AzureDevOps&prerelease

//////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////

var target = Argument("target", "Default");

//////////////////////////////////////////////////
// TARGETS
//////////////////////////////////////////////////

Task("Read-Build")
    .Does(() =>
{
    var build = AzureDevOpsBuildUsingAzurePipelinesOAuthToken();

    Information(build.BuildId);
});

Task("Read-BuildChanges")
    .Does(() =>
{
    var build = AzureDevOpsBuildUsingAzurePipelinesOAuthToken();

    var changes = build.GetChanges();
    if (!changes.Any())
    {
        Information("No changes found.");
    }
    else
    {
        foreach (var change in changes)
        {
            Information("{0}: {1} by {2}", change.Id, change.Message, change.Author);
        }
    }
});

Task("Read-BuildTimelineRecords")
    .Does(() =>
{
    var build = AzureDevOpsBuildUsingAzurePipelinesOAuthToken();

    var timelineRecords = build.GetTimelineRecords();
    if (!timelineRecords.Any())
    {
        Information("No timeline records found.");
    }
    else
    {
        foreach (var timelineRecord in timelineRecords)
        {
            Information("{0}: {1}", timelineRecord.Id, timelineRecord.Name);
        }
    }
});

Task("Read-BuildArtifacts")
    .Does(() =>
{
    // var build = AzureDevOpsBuildUsingAzurePipelinesOAuthToken();

    // var artifacts = build.GetArtifacts();
    // if (!artifacts.Any())
    // {
    //     Information("No artifacts found.");
    // }
    // else
    // {
    //     foreach (var artifact in artifacts)
    //     {
    //         Information("{0}: {1}", artifact.Id, artifact.Name);
    //     }
    // }
});

Task("Read-BuildTestRuns")
    .Does(() =>
{
    // var build = AzureDevOpsBuildUsingAzurePipelinesOAuthToken();

    // var testRuns = build.GetTestRuns();
    // if (!testRuns.Any())
    // {
    //     Information("No test runs found.");
    // }
    // else
    // {
    //     foreach (var testRun in testRuns)
    //     {
    //         Information("{0}", testRun.RunId);
    //     }
    // }
});

Task("Read-PullRequest")
    .WithCriteria((context) => context.BuildSystem().IsPullRequest, "Only supported for pull request builds.")
    .Does(() =>
{
    // var pullRequest =
    //     AzureDevOpsPullRequestUsingAzurePipelinesOAuthToken();

    // Information(pullRequest.TargetRefName);
});

Task("Read-BuildWorkItems")
    .Does(() =>
{
    // var build = AzureDevOpsBuildUsingAzurePipelinesOAuthToken();

    // var workItems = build.GetWorkItems();
    // if (!workItems.Any())
    // {
    //     Information("No work items found.");
    // }
    // else
    // {
    //     foreach (var workItem in workItems)
    //     {
    //         Information("{0}: {1}", workItem.WorkItemId, workItem.Title);
    //     }
    // }
});

Task("Default")
    .IsDependentOn("Read-Build")
    .IsDependentOn("Read-BuildChanges")
    .IsDependentOn("Read-BuildTimelineRecords")
    .IsDependentOn("Read-BuildArtifacts")
    .IsDependentOn("Read-BuildTestRuns")
    .IsDependentOn("Read-PullRequest")
    .IsDependentOn("Read-BuildWorkItems");

//////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////

RunTarget(target);