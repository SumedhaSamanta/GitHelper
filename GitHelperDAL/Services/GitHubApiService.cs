using GitHelperDAL.Model;
using System;
using System.Collections.Generic;

namespace GitHelperDAL.Services
{
    public abstract class GitHubApiService
    {
        public abstract bool AuthenticateUser();

        public abstract string GetAvtarUrl();

        public abstract List<RepoDetailsModel> GetRepoDetails();
        public abstract ParticularRepoDetailsModel GetParticularRepoDetails(string owner, string repositoryName);
        public abstract DateTimeOffset GetRepositoryCreationDate(string owner, string repositoryName);

        public abstract List<LanguageDetails> GetRepositoryLanguages(string owner, string repositoryName);

        public abstract List<CommitDetailsModel> GetCommitDetails(string owner, string repositoryName);

        public abstract int GetTotalNoOfCommits(string owner, string repositoryName);

        public abstract List<CommitDetailsModel> GetCommitsForInterval(string owner, string repositoryName, DateTimeOffset startTimeStamp, DateTimeOffset endTimeStamp);

        public abstract List<CommitDetailsModel> GetPaginatedCommits(string owner, string repositoryName, int pageNumber, int pageSize);


        //Factory Pattern Implementation
        public static GitHubApiService getInstance(string UserName, string Token)
        {
            return new OctoKitApiServiceImpl(UserName, Token);

        }


    }
}
