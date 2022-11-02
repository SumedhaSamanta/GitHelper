using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using GitHelperDAL.Model;
using GitHelperDAL.Services;
using Octokit;

namespace GitHelperDAL
{
    public class OctoKitApiServiceImpl : GitHubApiService
    {
        private GitHubClient clientDetail;

        private string userName;


        public OctoKitApiServiceImpl(string userName, string token)
        {
            this.clientDetail = new GitHubClient(new ProductHeaderValue("git-helper"));
            var tokenAuth = new Credentials(token);
            clientDetail.Credentials = tokenAuth;
            this.userName = userName;
        }

        //It authenticates user with userName and Personal Access Token
        public override bool AuthenticateUser()
        {
            try
            {

                var task = GetUser(clientDetail);
                User user = task.Result;
                if (user.Login != this.userName)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


        //It gets all the basic information of the user
        public static Task<User> GetUser(GitHubClient client)
        {
            var user = client.User.Current();
            return user;
        }


        // It gets the url of the GitHub Avatar
        public override string GetAvtarUrl()
        {
            var task = GetUser(clientDetail);
            User user = task.Result;

            return (user.AvatarUrl);

        }

        //It gets the list of repository name and owner
        public override List<RepoDetailsModel> GetRepoDetails()
        {
            var repositories = clientDetail.Repository.GetAllForCurrent().Result;
            List<RepoDetailsModel> repoList = new List<RepoDetailsModel>();
            foreach (var repository in repositories)
            {
                repoList.Add(new RepoDetailsModel { repoName = repository.Name, owner = repository.Owner.Login });
            }
            return repoList;

        }

        // It gets the details of the particular repository
        public override ParticularRepoDetailsModel GetParticularRepoDetails(string owner, string repositoryName)
        {
            var repository = clientDetail.Repository.Get(owner, repositoryName).Result;
            ParticularRepoDetailsModel particularRepoDetails = new ParticularRepoDetailsModel();
            particularRepoDetails.repoName = repository.Name;
            particularRepoDetails.repoLink = repository.SvnUrl;
            particularRepoDetails.owner = repository.Owner.Login;
            particularRepoDetails.createdAt = repository.CreatedAt.UtcDateTime;
            particularRepoDetails.updatedAt = repository.UpdatedAt.UtcDateTime;

            return particularRepoDetails;
        }

        //It gets the date of repository creation date
        public override DateTimeOffset GetRepositoryCreationDate(string owner, string repositoryName)
        {
            var repository = clientDetail.Repository.Get(owner, repositoryName).Result;
            DateTimeOffset date = repository.CreatedAt.UtcDateTime;
            return date;
        }

        //It gets the language of the particular repository
        public override List<LanguageDetails> GetRepositoryLanguages(string owner, string repositoryName)
        {
            List<LanguageDetails> languagesUsed = new List<LanguageDetails>();
            foreach(var language in clientDetail.Repository.GetAllLanguages(owner, repositoryName).Result)
            {
                languagesUsed.Add(new LanguageDetails { language = language.Name, bytesOfCode = language.NumberOfBytes });
            }
            return languagesUsed;
        }

        //It gets commit author name,commit message, commit date
        public override List<CommitDetailsModel> GetCommitDetails(string owner, string repositoryName)
        {
            List<CommitDetailsModel> commitMessages = new List<CommitDetailsModel>();
            var commits = clientDetail.Repository.Commit.GetAll(owner, repositoryName).Result;
            foreach (var commit in commits)
            {
                commitMessages.Add(new CommitDetailsModel { commitAuthorName = commit.Commit.Author.Name, commitMessage = commit.Commit.Message, commitDateTime = commit.Commit.Committer.Date }) ;
            }
            return commitMessages;
        }

        //It gets total no of commits of a repository
        public override int GetTotalNoOfCommits(string owner, string repositoryName)
        {
            int totalNoOfCommits = 0;
            var commits = clientDetail.Repository.Commit.GetAll(owner, repositoryName).Result;
            foreach (var cm in commits)
            {
                totalNoOfCommits++;
            }
            return totalNoOfCommits;
        }

        //It gets all the commits made within a time interval for a given repository
        public override List<CommitDetailsModel> GetCommitsForInterval(string owner, string repositoryName, DateTimeOffset startTimeStamp, DateTimeOffset endTimeStamp)
        {

            List<CommitDetailsModel> commitMessages = new List<CommitDetailsModel>();
            var request = new CommitRequest { Since = startTimeStamp, Until = endTimeStamp };
            var commitList = clientDetail.Repository.Commit.GetAll(owner, repositoryName, request).Result;
            foreach (var commit in commitList)
            {
                commitMessages.Add(new CommitDetailsModel { commitAuthorName = commit.Commit.Author.Name, commitMessage = commit.Commit.Message, commitDateTime = commit.Commit.Committer.Date });
            }
            return commitMessages;
        }

        //It gets all the commits of a given repository in paginated manner
        public override List<CommitDetailsModel> GetPaginatedCommits(string owner, string repositoryName, int pageNumber, int pageSize)
        {

            List<CommitDetailsModel> commitMessages = new List<CommitDetailsModel>();
            var request = new ApiOptions
            {
                PageSize = pageSize,
                StartPage = pageNumber, // 1-indexed value
                PageCount = 1
            };
            var commitList = clientDetail.Repository.Commit.GetAll(owner, repositoryName, request).Result;
            foreach (var commit in commitList)
            {
                commitMessages.Add(new CommitDetailsModel { commitAuthorName = commit.Commit.Author.Name, commitMessage = commit.Commit.Message, commitDateTime = commit.Commit.Committer.Date });
            }
            return commitMessages;
        }

    }
}
