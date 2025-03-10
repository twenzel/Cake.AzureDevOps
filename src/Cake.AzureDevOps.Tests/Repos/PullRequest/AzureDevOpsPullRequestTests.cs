﻿namespace Cake.AzureDevOps.Tests.Repos.PullRequest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cake.AzureDevOps.Repos.PullRequest;
    using Cake.AzureDevOps.Repos.PullRequest.CommentThread;
    using Cake.AzureDevOps.Tests.Fakes;
    using Cake.Core.Diagnostics;
    using Cake.Core.IO;
    using Microsoft.VisualStudio.Services.Common;
    using Shouldly;
    using Xunit;

    public sealed class AzureDevOpsPullRequestTests
    {
        public sealed class TheCtor
        {
            [Fact]
            public void Should_Throw_If_Log_Is_Null()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, "foo") { Log = null };

                // When
                var result = Record.Exception(() => new AzureDevOpsPullRequest(fixture.Log, fixture.Settings));

                // Then
                result.IsArgumentNullException("log");
            }

            [Fact]
            public void Should_Throw_If_Settings_Are_Null()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 42) { Settings = null };

                // When
                var result = Record.Exception(() => new AzureDevOpsPullRequest(fixture.Log, fixture.Settings));

                // Then
                result.IsArgumentNullException("settings");
            }

            [Fact]
            public void Should_Throw_If_AzureDevOps_Url_Is_Invalid()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.InvalidUrl, 42);

                // When
                var result = Record.Exception(() => new AzureDevOpsPullRequest(fixture.Log, fixture.Settings));

                // Then
                result.IsUrlFormatException();
            }
        }

        public sealed class TheCtorWithGitClientFactory
        {
            [Fact]
            public void Should_Throw_If_Log_Is_Null()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, "foo") { Log = null };

                // When
                var result = Record.Exception(() => new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory));

                // Then
                result.IsArgumentNullException("log");
            }

            [Fact]
            public void Should_Throw_If_Settings_Are_Null()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 42) { Settings = null };

                // When
                var result = Record.Exception(() => new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory));

                // Then
                result.IsArgumentNullException("settings");
            }

            [Fact]
            public void Should_Throw_If_Git_Client_Factory_Is_Null()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 42) { GitClientFactory = null };

                // When
                var result = Record.Exception(() => new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory));

                // Then
                result.IsArgumentNullException("gitClientFactory");
            }

            [Fact]
            public void Should_Throw_If_AzureDevOps_Url_Is_Invalid()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.InvalidUrl, 42);

                // When
                var result = Record.Exception(() => new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory));

                // Then
                result.IsUrlFormatException();
            }

            [Fact]
            public void Should_Return_Valid_AzureDevOps_Pull_Request_By_Id()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 42);

                // When
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // Then
                pullRequest.ShouldNotBe(null);
                pullRequest.HasPullRequestLoaded.ShouldBe(true);
                pullRequest.PullRequestId.ShouldBe(42);
                pullRequest.PullRequestStatus.ShouldBe(AzureDevOpsPullRequestState.Active);
                pullRequest.RepositoryName.ShouldBe("MyRepoName");
                pullRequest.CollectionName.ShouldBe("MyCollection");
                pullRequest.CodeReviewId.ShouldBe(123);
                pullRequest.ProjectName.ShouldBe("MyTeamProject");
                pullRequest.SourceRefName.ShouldBe("foo");
                pullRequest.TargetRefName.ShouldBe("master");
                pullRequest.LastSourceCommitId.ShouldBe("4a92b977");
                pullRequest.LastTargetCommitId.ShouldBe("78a3c113");
            }

            [Fact]
            public void Should_Return_Valid_Azure_DevOps_Pull_Request_By_Id()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsUrl, 16);

                // When
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // Then
                pullRequest.ShouldNotBe(null);
                pullRequest.HasPullRequestLoaded.ShouldBe(true);
                pullRequest.PullRequestId.ShouldBe(16);
                pullRequest.PullRequestStatus.ShouldBe(AzureDevOpsPullRequestState.Active);
                pullRequest.RepositoryName.ShouldBe("MyRepoName");
                pullRequest.CollectionName.ShouldBe("DefaultCollection");
                pullRequest.CodeReviewId.ShouldBe(123);
                pullRequest.ProjectName.ShouldBe("MyProject");
                pullRequest.SourceRefName.ShouldBe("foo");
                pullRequest.TargetRefName.ShouldBe("master");
                pullRequest.LastSourceCommitId.ShouldBe("4a92b977");
                pullRequest.LastTargetCommitId.ShouldBe("78a3c113");
            }

            [Fact]
            public void Should_Return_Valid_AzureDevOps_Pull_Request_By_Source_Branch()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, "feature");

                // When
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // Then
                pullRequest.ShouldNotBe(null);
                pullRequest.HasPullRequestLoaded.ShouldBe(true);
                pullRequest.PullRequestId.ShouldBe(777);
                pullRequest.PullRequestStatus.ShouldBe(AzureDevOpsPullRequestState.Active);
                pullRequest.RepositoryName.ShouldBe("MyRepoName");
                pullRequest.CollectionName.ShouldBe("MyCollection");
                pullRequest.CodeReviewId.ShouldBe(123);
                pullRequest.ProjectName.ShouldBe("MyTeamProject");
                pullRequest.SourceRefName.ShouldBe("feature");
                pullRequest.TargetRefName.ShouldBe("master");
                pullRequest.LastSourceCommitId.ShouldBe("4a92b977");
                pullRequest.LastTargetCommitId.ShouldBe("78a3c113");
            }

            [Fact]
            public void Should_Return_Valid_Azure_DevOps_Pull_Request_By_Source_Branch()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsUrl, "feature");

                // When
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // Then
                pullRequest.ShouldNotBe(null);
                pullRequest.HasPullRequestLoaded.ShouldBe(true);
                pullRequest.PullRequestId.ShouldBe(777);
                pullRequest.PullRequestStatus.ShouldBe(AzureDevOpsPullRequestState.Active);
                pullRequest.RepositoryName.ShouldBe("MyRepoName");
                pullRequest.CollectionName.ShouldBe("DefaultCollection");
                pullRequest.CodeReviewId.ShouldBe(123);
                pullRequest.ProjectName.ShouldBe("MyProject");
                pullRequest.SourceRefName.ShouldBe("feature");
                pullRequest.TargetRefName.ShouldBe("master");
                pullRequest.LastSourceCommitId.ShouldBe("4a92b977");
                pullRequest.LastTargetCommitId.ShouldBe("78a3c113");
            }

            [Fact]
            public void Should_Return_Null_AzureDevOps_Pull_Request_By_Id()
            {
                // Given
                var fixture =
                    new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 101)
                    {
                        GitClientFactory = new FakeNullGitClientFactory(),
                        Settings = { ThrowExceptionIfPullRequestCouldNotBeFound = false },
                    };

                // When
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // Then
                pullRequest.ShouldNotBe(null);
                pullRequest.HasPullRequestLoaded.ShouldBe(false);
                pullRequest.RepositoryName.ShouldBe("MyRepoName");
                pullRequest.CollectionName.ShouldBe("MyCollection");
                pullRequest.ProjectName.ShouldBe("MyTeamProject");
                pullRequest.PullRequestId.ShouldBe(0);
                pullRequest.PullRequestStatus.ShouldBe(AzureDevOpsPullRequestState.NotSet);
                pullRequest.CodeReviewId.ShouldBe(0);
                pullRequest.SourceRefName.ShouldBeEmpty();
                pullRequest.TargetRefName.ShouldBeEmpty();
                pullRequest.LastSourceCommitId.ShouldBeEmpty();
                pullRequest.LastTargetCommitId.ShouldBeEmpty();
            }

            [Fact]
            public void Should_Return_Null_Azure_DevOps_Pull_Request_By_Id()
            {
                // Given
                var fixture =
                    new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsUrl, 101)
                    {
                        GitClientFactory = new FakeNullGitClientFactory(),
                        Settings = { ThrowExceptionIfPullRequestCouldNotBeFound = false },
                    };

                // When
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // Then
                pullRequest.ShouldNotBe(null);
                pullRequest.HasPullRequestLoaded.ShouldBe(false);
                pullRequest.RepositoryName.ShouldBe("MyRepoName");
                pullRequest.CollectionName.ShouldBe("DefaultCollection");
                pullRequest.ProjectName.ShouldBe("MyProject");
                pullRequest.PullRequestId.ShouldBe(0);
                pullRequest.PullRequestStatus.ShouldBe(AzureDevOpsPullRequestState.NotSet);
                pullRequest.CodeReviewId.ShouldBe(0);
                pullRequest.SourceRefName.ShouldBeEmpty();
                pullRequest.TargetRefName.ShouldBeEmpty();
                pullRequest.LastSourceCommitId.ShouldBeEmpty();
                pullRequest.LastTargetCommitId.ShouldBeEmpty();
            }

            [Fact]
            public void Should_Return_Null_AzureDevOps_Pull_Request_By_Branch()
            {
                // Given
                var fixture =
                    new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, "somebranch")
                    {
                        GitClientFactory = new FakeNullGitClientFactory(),
                        Settings = { ThrowExceptionIfPullRequestCouldNotBeFound = false },
                    };

                // When
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // Then
                pullRequest.ShouldNotBe(null);
                pullRequest.HasPullRequestLoaded.ShouldBe(false);
                pullRequest.RepositoryName.ShouldBe("MyRepoName");
                pullRequest.CollectionName.ShouldBe("MyCollection");
                pullRequest.ProjectName.ShouldBe("MyTeamProject");
                pullRequest.PullRequestId.ShouldBe(0);
                pullRequest.PullRequestStatus.ShouldBe(AzureDevOpsPullRequestState.NotSet);
                pullRequest.CodeReviewId.ShouldBe(0);
                pullRequest.SourceRefName.ShouldBeEmpty();
                pullRequest.TargetRefName.ShouldBeEmpty();
                pullRequest.LastSourceCommitId.ShouldBeEmpty();
                pullRequest.LastTargetCommitId.ShouldBeEmpty();
            }

            [Fact]
            public void Should_Return_Null_Azure_DevOps_Pull_Request_By_Branch()
            {
                // Given
                var fixture =
                    new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsUrl, "somebranch")
                    {
                        GitClientFactory = new FakeNullGitClientFactory(),
                        Settings = { ThrowExceptionIfPullRequestCouldNotBeFound = false },
                    };

                // When
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // Then
                pullRequest.ShouldNotBe(null);
                pullRequest.HasPullRequestLoaded.ShouldBe(false);
                pullRequest.RepositoryName.ShouldBe("MyRepoName");
                pullRequest.CollectionName.ShouldBe("DefaultCollection");
                pullRequest.ProjectName.ShouldBe("MyProject");
                pullRequest.PullRequestId.ShouldBe(0);
                pullRequest.PullRequestStatus.ShouldBe(AzureDevOpsPullRequestState.NotSet);
                pullRequest.CodeReviewId.ShouldBe(0);
                pullRequest.SourceRefName.ShouldBeEmpty();
                pullRequest.TargetRefName.ShouldBeEmpty();
                pullRequest.LastSourceCommitId.ShouldBeEmpty();
                pullRequest.LastTargetCommitId.ShouldBeEmpty();
            }

            [Fact]
            public void Should_Throw_If_Strict_Is_On_And_Pull_Request_Is_Null_By_Id()
            {
                // Given
                var fixture =
                    new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 1)
                    {
                        GitClientFactory = new FakeNullGitClientFactory(),
                    };

                // When
                var result = Record.Exception(() => new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory));

                // Then
                result.IsAzureDevOpsPullRequestNotFoundException();
            }

            [Fact]
            public void Should_Throw_If_Strict_Is_On_And_Pull_Request_Is_Null_By_Branch()
            {
                // Given
                var fixture =
                    new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, "feature")
                    {
                        GitClientFactory = new FakeNullGitClientFactory(),
                    };

                // When
                var result = Record.Exception(() => new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory));

                // Then
                result.IsAzureDevOpsPullRequestNotFoundException();
            }
        }

        public sealed class TheCreateMethod
        {
            [Fact]
            public void Should_Throw_If_Log_Is_Null()
            {
                // Given
                var fixture =
                    new CreatePullRequestFixture(
                        BasePullRequestFixture.ValidAzureDevOpsServerUrl,
                        "testBranch",
                        "NotExistingBranch",
                        "test",
                        "test");
                ICakeLog log = null;

                // When
                var result =
                    Record.Exception(() => AzureDevOpsPullRequest.Create(log, fixture.GitClientFactory, fixture.Settings));

                // Then
                result.IsArgumentNullException("log");
            }

            [Fact]
            public void Should_Throw_If_GitClientFactory_Is_Null()
            {
                // Given
                var fixture =
                    new CreatePullRequestFixture(
                        BasePullRequestFixture.ValidAzureDevOpsServerUrl,
                        "testBranch",
                        "NotExistingBranch",
                        "test",
                        "test");
                IGitClientFactory gitClientFactory = null;

                // When
                var result =
                    Record.Exception(() => AzureDevOpsPullRequest.Create(fixture.Log, gitClientFactory, fixture.Settings));

                // Then
                result.IsArgumentNullException("gitClientFactory");
            }

            [Fact]
            public void Should_Throw_If_Settings_Are_Null()
            {
                // Given
                var fixture =
                    new CreatePullRequestFixture(
                        BasePullRequestFixture.ValidAzureDevOpsServerUrl,
                        "testBranch",
                        "NotExistingBranch",
                        "test",
                        "test");
                AzureDevOpsCreatePullRequestSettings settings = null;

                // When
                var result =
                    Record.Exception(() => AzureDevOpsPullRequest.Create(fixture.Log, fixture.GitClientFactory, settings));

                // Then
                result.IsArgumentNullException("settings");
            }

            [Fact]
            public void Should_Throw_If_Target_Branch_Not_Found()
            {
                // Given
                var fixture =
                    new CreatePullRequestFixture(
                        BasePullRequestFixture.ValidAzureDevOpsServerUrl,
                        "testBranch",
                        "NotExistingBranch",
                        "test",
                        "test");

                // When
                var result =
                    Record.Exception(() => AzureDevOpsPullRequest.Create(fixture.Log, fixture.GitClientFactory, fixture.Settings));

                // Then
                result.IsAzureDevOpsBranchNotFoundException($"Branch not found \"NotExistingBranch\"");
            }

            [Fact]
            public void Should_Return_A_PullRequest()
            {
                // Given
                var sourceRefName = "testBranch";
                var targetRefName = "master";
                var title = "foo";
                var description = "bar";
                var fixture =
                    new CreatePullRequestFixture(
                        BasePullRequestFixture.ValidAzureDevOpsServerUrl,
                        sourceRefName,
                        targetRefName,
                        title,
                        description);

                // When
                AzureDevOpsPullRequest.Create(fixture.Log, fixture.GitClientFactory, fixture.Settings);

                // Then
                // Return is a mocked pull request unrelated to the input values
            }

            [Fact]
            public void Should_Return_A_PullRequest_With_Fallback_To_Default_Target_Branch()
            {
                // Given
                var sourceRefName = "testBranch";
                string targetRefName = null;
                var title = "foo";
                var description = "bar";
                var fixture =
                    new CreatePullRequestFixture(
                        BasePullRequestFixture.ValidAzureDevOpsServerUrl,
                        sourceRefName,
                        targetRefName,
                        title,
                        description);

                // When
                AzureDevOpsPullRequest.Create(fixture.Log, fixture.GitClientFactory, fixture.Settings);

                // Then
                // Return is a mocked pull request unrelated to the input values
            }
        }

        public sealed class TheVoteMethod
        {
            [Fact]
            public void Should_Set_Approved_Vote_On_AzureDevOps_Pull_Request()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 23);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                pullRequest.Vote(AzureDevOpsPullRequestVote.Approved);

                // Then
                // ?? Nothing to validate here since the method returns void
            }

            [Fact]
            public void Should_Throw_If_Vote_Value_Is_Invalid_On_AzureDevOps_Pull_Request()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 23);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var result = Record.Exception(() => pullRequest.Vote((AzureDevOpsPullRequestVote)3));

                // Then
                result.ShouldNotBe(null);
                result.IsExpected("Vote");
                result.Message.ShouldBe("Something went wrong");
            }

            [Fact]
            public void Should_Throw_If_Null_Is_Returned_On_AzureDevOps_Pull_Request()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 23)
                {
                    GitClientFactory = new FakeNullForMethodsGitClientFactory(),
                };
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var result = Record.Exception(() => pullRequest.Vote(AzureDevOpsPullRequestVote.WaitingForAuthor));

                // Then
                result.ShouldNotBe(null);
                result.IsExpected("Vote");
                result.IsAzureDevOpsPullRequestNotFoundException();
            }
        }

        public sealed class TheSetStatusMethod
        {
            [Fact]
            public void Should_Throw_If_AzureDevOps_Pull_Request_Status_Is_Null()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 16);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var result = Record.Exception(() => pullRequest.SetStatus(null));

                // Then
                result.IsArgumentNullException("status");
            }

            [Fact]
            public void Should_Throw_If_AzureDevOps_Pull_Request_State_Is_Invalid()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 16);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);
                var status = new AzureDevOpsPullRequestStatus("whatever") { State = (AzureDevOpsPullRequestStatusState)123 };

                // When
                var result = Record.Exception(() => pullRequest.SetStatus(status));

                // Then
                result.ShouldNotBe(null);
                result.IsExpected("SetStatus");
                result.Message.ShouldBe("Unknown value");
            }

            [Fact]
            public void Should_Set_Valid_Status_On_AzureDevOps_Pull_Request()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 16);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);
                var status = new AzureDevOpsPullRequestStatus("Hello") { State = AzureDevOpsPullRequestStatusState.Succeeded };

                // When
                pullRequest.SetStatus(status);

                // Then
                // ?? Nothing to validate here since the method returns void
            }

            [Fact]
            public void Should_Throw_If_Null_Is_Returned_On_AzureDevOps_Pull_Request()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 16)
                {
                    GitClientFactory = new FakeNullForMethodsGitClientFactory(),
                };
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);
                var status = new AzureDevOpsPullRequestStatus("One") { State = AzureDevOpsPullRequestStatusState.Failed };

                // When
                var result = Record.Exception(() => pullRequest.SetStatus(status));

                // Then
                result.ShouldNotBe(null);
                result.IsExpected("SetStatus");
                result.IsAzureDevOpsPullRequestNotFoundException();
            }
        }

        public sealed class TheGetModifiedFilesMethod
        {
            [Fact]
            public void Should_Return_Empty_Collection_If_No_Changes_Found()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 42) { GitClientFactory = new FakeNullForMethodsGitClientFactory() };
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var files = pullRequest.GetModifiedFiles();

                // Then
                files.ShouldBeOfType<List<FilePath>>();
                files.ShouldNotBeNull();
                files.ShouldBeEmpty();
            }

            [Fact]
            public void Should_Return_Valid_Collection_Of_Modified_Files()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 42);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var files = pullRequest.GetModifiedFiles();

                // Then
                files.ShouldNotBeNull();
                files.ShouldNotBeEmpty();
                files.ShouldHaveSingleItem();

                var filePath = files.First();
                filePath.ShouldBeOfType<FilePath>();
                filePath.ShouldNotBeNull();
                filePath.FullPath.ShouldNotBeEmpty();
                filePath.FullPath.ShouldBe("src/project/myclass.cs");
            }
        }

        public sealed class TheSetCommentThreadStatusMethod
        {
            [Fact]
            public void Should_Activate_Comment_Thread()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 12);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                pullRequest.ActivateCommentThread(321);

                // Then
                // ?? Nothing to validate here since the method returns void
            }

            [Fact]
            public void Should_Resolve_Comment_Thread()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsUrl, 21);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                pullRequest.ResolveCommentThread(123);

                // Then
                // ?? Nothing to validate here since the method returns void
            }

            [Fact]
            public void Should_Resolve_Close_Thread()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsUrl, 21);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                pullRequest.CloseCommentThread(123);

                // Then
                // ?? Nothing to validate here since the method returns void
            }

            [Fact]
            public void Should_Not_Throw_If_Null_Is_Returned()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 11)
                {
                    GitClientFactory = new FakeNullForMethodsGitClientFactory(),
                };
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                pullRequest.ActivateCommentThread(35);
                pullRequest.ResolveCommentThread(53);

                // Then
                // ?? Nothing to validate here since the method returns void
            }
        }

        public sealed class TheGetCommentThreadsMethod
        {
            [Fact]
            public void Should_Not_Fail_If_Empty_List_Is_Returned()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 33)
                {
                    GitClientFactory = new FakeNullForMethodsGitClientFactory(),
                };
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var threads = pullRequest.GetCommentThreads();

                // Then
                threads.ShouldNotBeNull();
                threads.ShouldBeEmpty();
            }

            [Fact]
            public void Should_Return_Valid_Comment_Threads()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsUrl, 44);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var threads = pullRequest.GetCommentThreads();

                // Then
                threads.ShouldNotBeNull();
                threads.ShouldNotBeEmpty();
                threads.Count().ShouldBe(2);

                AzureDevOpsPullRequestCommentThread thread1 = threads.First();
                thread1.Id.ShouldBe(11);
                thread1.Status.ShouldBe(AzureDevOpsCommentThreadStatus.Active);
                thread1.FilePath.ShouldNotBeNull();
                thread1.FilePath.FullPath.ShouldBe("some/path/to/file.cs");

                thread1.Comments.ShouldNotBeNull();
                thread1.Comments.ShouldNotBeEmpty();
                thread1.Comments.Count().ShouldBe(2);

                AzureDevOpsComment comment11 = thread1.Comments.First();
                comment11.ShouldNotBeNull();
                comment11.Content.ShouldBe("Hello");
                comment11.IsDeleted.ShouldBe(false);
                comment11.CommentType.ShouldBe(AzureDevOpsCommentType.CodeChange);
                ((int)comment11.Id).ShouldBeGreaterThan(0);

                AzureDevOpsComment comment12 = thread1.Comments.Last();
                comment12.ShouldNotBeNull();
                comment12.Content.ShouldBe("Goodbye");
                comment12.IsDeleted.ShouldBe(true);
                comment12.CommentType.ShouldBe(AzureDevOpsCommentType.Text);
                ((int)comment12.Id).ShouldBeGreaterThan(0);

                AzureDevOpsPullRequestCommentThread thread2 = threads.Last();
                thread2.Id.ShouldBe(22);
                thread2.Status.ShouldBe(AzureDevOpsCommentThreadStatus.Fixed);
                thread2.FilePath.ShouldBeNull();
                thread2.Comments.ShouldNotBeNull();
                thread2.Comments.ShouldBeEmpty();
            }
        }

        public sealed class TheCreateCommentMethod
        {
            [Theory]
            [InlineData((string)null, typeof(ArgumentNullException))]
            [InlineData("", typeof(ArgumentOutOfRangeException))]
            [InlineData(" ", typeof(ArgumentOutOfRangeException))]
            public void Should_Throw_If_Comment_Is_Null_Or_Empty_Or_Whitespace(string comment, Type expectedExceptionType)
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 100);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var result = Record.Exception(() => pullRequest.CreateComment(comment)) as ArgumentException;

                // Then
                result.ShouldNotBeNull();
                result.IsArgumentException(expectedExceptionType, "comment");
            }

            [Fact]
            public void Should_Return_Null_If_Null_Is_Returned_From_Git_Client()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 100)
                {
                    GitClientFactory = new FakeNullForMethodsGitClientFactory(),
                };
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var thread = pullRequest.CreateComment("Foo");

                // Then
                thread.ShouldBeNull();
            }

            [Fact]
            public void Should_Create_Valid_Thread_With_One_Comment()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 100);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var thread = pullRequest.CreateComment("Valid");

                // Then
                thread.ShouldNotBeNull();
                thread.Status.ShouldBe(AzureDevOpsCommentThreadStatus.Active);
                thread.Comments.ShouldNotBeNull();
                thread.Comments.Count().ShouldBe(1);

                var comment = thread.Comments.First();
                comment.CommentType.ShouldBe(AzureDevOpsCommentType.System);
                comment.IsDeleted.ShouldBeFalse();
                comment.Content.ShouldBe("Valid");
            }
        }

        public sealed class TheCreateCommentWithFileMethod
        {
            [Theory]
            [InlineData((string)null, typeof(ArgumentNullException))]
            [InlineData("", typeof(ArgumentOutOfRangeException))]
            [InlineData(" ", typeof(ArgumentOutOfRangeException))]
            public void Should_Throw_If_Comment_Is_Null_Or_Empty_Or_Whitespace(string comment, Type expectedExceptionType)
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 100);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var result = Record.Exception(() => pullRequest.CreateComment(comment, new FilePath("text.txt"), 0, 0)) as ArgumentException;

                // Then
                result.ShouldNotBeNull();
                result.IsArgumentException(expectedExceptionType, "comment");
            }

            [Theory]
            [InlineData((string)null, typeof(ArgumentNullException))]
            [InlineData("", typeof(ArgumentOutOfRangeException))]
            [InlineData(" ", typeof(ArgumentOutOfRangeException))]
            public void Should_Throw_If_FilePath_Is_Null_Or_Empty_Or_Whitespace(string filePath, Type expectedExceptionType)
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 100);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var result = Record.Exception(() => pullRequest.CreateComment("test", filePath == null ? null : new FilePath(filePath), 0, 0)) as ArgumentException;

                // Then
                result.ShouldNotBeNull();
                result.IsArgumentException(expectedExceptionType, "filePath");
            }

            [Theory]
            [InlineData(0, typeof(ArgumentOutOfRangeException))]
            [InlineData(-1, typeof(ArgumentOutOfRangeException))]
            [InlineData(-50, typeof(ArgumentOutOfRangeException))]
            public void Should_Throw_If_LineNumber_Is_Negative_Or_Zeror(int lineNumber, Type expectedExceptionType)
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 100);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var result = Record.Exception(() => pullRequest.CreateComment("test", new FilePath("test.txt"), lineNumber, 0)) as ArgumentException;

                // Then
                result.ShouldNotBeNull();
                result.IsArgumentException(expectedExceptionType, "lineNumber");
            }

            [Theory]
            [InlineData(-1, typeof(ArgumentOutOfRangeException))]
            [InlineData(-50, typeof(ArgumentOutOfRangeException))]
            public void Should_Throw_If_Offset_Is_Negative(int offset, Type expectedExceptionType)
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 100);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var result = Record.Exception(() => pullRequest.CreateComment("test", new FilePath("test.txt"), 1, offset)) as ArgumentException;

                // Then
                result.ShouldNotBeNull();
                result.IsArgumentException(expectedExceptionType, "offset");
            }

            [Fact]
            public void Should_Return_Null_If_Null_Is_Returned_From_Git_Client()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 100)
                {
                    GitClientFactory = new FakeNullForMethodsGitClientFactory(),
                };
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var thread = pullRequest.CreateComment("Foo", new FilePath("test.txt"), 10, 50);

                // Then
                thread.ShouldBeNull();
            }

            [Fact]
            public void Should_Create_Valid_Thread_With_One_Comment()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 100);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var thread = pullRequest.CreateComment("Valid", new FilePath("src/test.txt"), 10, 50);

                // Then
                thread.ShouldNotBeNull();
                thread.Status.ShouldBe(AzureDevOpsCommentThreadStatus.Active);
                thread.FilePath.FullPath.ShouldBe("src/test.txt");
                thread.LineNumber.ShouldBe(10);
                thread.Offset.ShouldBe(50);

                thread.Comments.ShouldNotBeNull();
                thread.Comments.Count().ShouldBe(1);

                var comment = thread.Comments.First();
                comment.CommentType.ShouldBe(AzureDevOpsCommentType.System);
                comment.IsDeleted.ShouldBeFalse();
                comment.Content.ShouldBe("Valid");
            }
        }

        public sealed class TheDeleteCommentMethod
        {
            [Theory]
            [InlineData(0, typeof(ArgumentOutOfRangeException))]
            [InlineData(-1, typeof(ArgumentOutOfRangeException))]
            [InlineData(-55, typeof(ArgumentOutOfRangeException))]
            public void Should_Throw_If_ThreadId_Is_Zero_Or_Below(int threadId, Type expectedExceptionType)
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 100);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var result = Record.Exception(() => pullRequest.DeleteComment(threadId, 5)) as ArgumentException;

                // Then
                result.ShouldNotBeNull();
                result.IsArgumentException(expectedExceptionType, "threadId");
            }

            [Theory]
            [InlineData(0, typeof(ArgumentOutOfRangeException))]
            [InlineData(-1, typeof(ArgumentOutOfRangeException))]
            [InlineData(-55, typeof(ArgumentOutOfRangeException))]
            public void Should_Throw_If_CommentId_Is_Zero_Or_Below(int commentId, Type expectedExceptionType)
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 100);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var result = Record.Exception(() => pullRequest.DeleteComment(5, commentId)) as ArgumentException;

                // Then
                result.ShouldNotBeNull();
                result.IsArgumentException(expectedExceptionType, "commentId");
            }

            [Fact]
            public void Should_Delete_Comment()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 100);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                pullRequest.DeleteComment(5, 1);

                // Then
                // ?? Nothing to validate here since the method returns void
            }

            [Fact]
            public void Should_Throw_If_Comment_Is_Null()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 100);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var result = Record.Exception(() => pullRequest.DeleteComment(null)) as ArgumentException;

                // Then
                result.ShouldNotBeNull();
                result.IsArgumentNullException("comment");
            }

            [Fact]
            public void Should_Delete_Comment_By_Comment_Properties()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 100);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);
                var inComment = new AzureDevOpsComment(new Microsoft.TeamFoundation.SourceControl.WebApi.Comment { Id = 1 }, 5)
                {
                    Content = "new Content",
                };

                // When
                pullRequest.DeleteComment(inComment);

                // Then
                // ?? Nothing to validate here since the method returns void
            }
        }

        public sealed class TheUpdateCommentMethod
        {
            [Fact]
            public void Should_Throw_If_Comment_Is_Null()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 100);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var result = Record.Exception(() => pullRequest.UpdateComment(null)) as ArgumentException;

                // Then
                result.ShouldNotBeNull();
                result.IsArgumentNullException("comment");
            }

            [Fact]
            public void Should_Return_Null_If_Pull_Request_Is_Invalid()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 100)
                {
                    GitClientFactory = new FakeNullGitClientFactory(),
                };
                fixture.Settings.ThrowExceptionIfPullRequestCouldNotBeFound = false;

                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var outThread = pullRequest.UpdateComment(new AzureDevOpsComment());

                // Then
                outThread.ShouldBeNull();
            }

            [Fact]
            public void Should_Return_Null_If_Null_Is_Returned_From_Git_Client()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 100)
                {
                    GitClientFactory = new FakeNullForMethodsGitClientFactory(),
                };

                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var outThread = pullRequest.UpdateComment(new AzureDevOpsComment());

                // Then
                outThread.ShouldBeNull();
            }

            [Fact]
            public void Should_Return_Updated_Comment()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsUrl, 200);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);
                var inComment = new AzureDevOpsComment(new Microsoft.TeamFoundation.SourceControl.WebApi.Comment { Id = 1 }, 5)
                {
                    Content = "new Content",
                };

                // When
                var outComment = pullRequest.UpdateComment(inComment);

                // Then
                outComment.Id.ShouldBe(inComment.Id);
                outComment.Content.ShouldBe(inComment.Content);
            }
        }

        public sealed class TheCreateCommentThreadMethod
        {
            [Fact]
            public void Should_Throw_If_Input_Thread_Is_Null()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 100);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var result = Record.Exception(() => pullRequest.CreateCommentThread(null));

                // Then
                result.IsArgumentNullException("thread");
            }

            [Fact]
            public void Should_Return_Null_If_Pull_Request_Is_Invalid()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 100)
                {
                    GitClientFactory = new FakeNullGitClientFactory(),
                };
                fixture.Settings.ThrowExceptionIfPullRequestCouldNotBeFound = false;

                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var outThread = pullRequest.CreateCommentThread(new AzureDevOpsPullRequestCommentThread());

                // Then
                outThread.ShouldBeNull();
            }

            [Fact]
            public void Should_Return_Null_If_Null_Is_Returned_From_Git_Client()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 100)
                {
                    GitClientFactory = new FakeNullForMethodsGitClientFactory(),
                };

                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var outThread = pullRequest.CreateCommentThread(new AzureDevOpsPullRequestCommentThread());

                // Then
                outThread.ShouldBeNull();
            }

            [Fact]
            public void Should_Create_Valid_Comment_Thread()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsUrl, 200);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);
                var inThread = new AzureDevOpsPullRequestCommentThread { Id = 300, Status = AzureDevOpsCommentThreadStatus.Pending, FilePath = "/index.html" };

                // When
                var outThread = pullRequest.CreateCommentThread(inThread);

                // Then
                outThread.Id.ShouldBe(inThread.Id);
                outThread.Status.ShouldBe(inThread.Status);
                outThread.FilePath.ShouldBeEquivalentTo(inThread.FilePath);
            }
        }

        public sealed class TheGetLatestIterationIdMethod
        {
            [Fact]
            public void Should_Throw_If_Null_Is_Returned()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 11)
                {
                    GitClientFactory = new FakeNullForMethodsGitClientFactory(),
                };
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var result = Record.Exception(() => pullRequest.GetLatestIterationId());

                // Then
                result.IsAzureDevOpsException();
            }

            [Fact]
            public void Should_Return_Valid_Iteration_Id()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsUrl, 12);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                int id = pullRequest.GetLatestIterationId();

                // Then
                id.ShouldBe(42);
            }

            [Fact]
            public void Should_Return_Invalid_Id_If_Something_Is_Wrong_With_Iteration()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 13);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                int id = pullRequest.GetLatestIterationId();

                // Then
                id.ShouldBe(-1);
            }
        }

        public sealed class TheGetIterationChangesMethod
        {
            [Fact]
            public void Should_Not_Throw_If_Null_Is_Returned()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsUrl, 21)
                {
                    GitClientFactory = new FakeNullForMethodsGitClientFactory(),
                };
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var changes = pullRequest.GetIterationChanges(42);

                // Then
                changes.ShouldBeNull();
            }

            [Fact]
            public void Should_Return_Collection_Of_Valid_Iteration_Changes()
            {
                // Given
                var fixture = new PullRequestFixture(BasePullRequestFixture.ValidAzureDevOpsServerUrl, 22);
                var pullRequest = new AzureDevOpsPullRequest(fixture.Log, fixture.Settings, fixture.GitClientFactory);

                // When
                var changes = pullRequest.GetIterationChanges(500);

                // Then
                changes.ShouldNotBeNull();
                changes.ShouldNotBeEmpty();
                changes.Count().ShouldBe(2);

                changes.First().ShouldNotBeNull();
                changes.First().ShouldBeOfType<AzureDevOpsPullRequestIterationChange>();
                changes.First().ChangeId.ShouldBe(100);
                changes.First().ChangeTrackingId.ShouldBe(1);
                changes.First().ItemPath.ShouldBeOfType<FilePath>();
                changes.First().ItemPath.FullPath.ShouldBe("/src/my/class1.cs");

                changes.Skip(1).First().ShouldNotBeNull();
                changes.Skip(1).First().ShouldBeOfType<AzureDevOpsPullRequestIterationChange>();
                changes.Skip(1).First().ChangeId.ShouldBe(200);
                changes.Skip(1).First().ChangeTrackingId.ShouldBe(2);
                changes.Skip(1).First().ItemPath.ShouldBeNull();
            }
        }
    }
}
