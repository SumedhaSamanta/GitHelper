using GitHelperDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHelperDAL.Services
{
    public abstract class GitHubApiService
    {
        public abstract bool AuthenticateUser();

        public abstract string GetAvtarUrl();

        public abstract List<RepoDetailsModel> GetRepoDetails();

        public abstract string GetRepositoryCreationDate(string Owner, string RepositoryName);

        public abstract List<LanguageDetails> GetRepositoryLanguages(string Owner, string RepositoryName);

        public abstract List<CommitDetailsModel> GetCommitDetails(string Owner, string RepositoryName); // Not implementing pagination

        public abstract int GetTotalNoOfCommits(string Owner, string RepositoryName); //Check use (not the full version)


        //Factory Pattern Implementation
        public static GitHubApiService getInstance(string UserName, string Token)
        {
            return new OctoKitApiServiceImpl(UserName, Token);

        }


    }
}
