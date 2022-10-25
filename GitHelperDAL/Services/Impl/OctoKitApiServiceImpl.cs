using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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


        public OctoKitApiServiceImpl(string UserName, string Token)
        {
            this.clientDetail = new GitHubClient(new ProductHeaderValue("git-helper"));
            var tokenAuth = new Credentials(Token);
            clientDetail.Credentials = tokenAuth;
            this.userName = UserName;
        }

        //It authenticate User with userName and Personal Access Token
        public override bool AuthenticateUser()
        {
            try
            {

                var task = GetUser(clientDetail);
                User user = task.Result;
                var repositories = clientDetail.Repository.GetAllForCurrent().Result;
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


        // It gets the url of the GitHub Avtar
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

        //It gets the date of repository creation date
        public override string GetRepositoryCreationDate(string Owner, string RepositoryName)
        {
            var creationDate = clientDetail.Repository.Get(Owner, RepositoryName).Result;
            string date = creationDate.CreatedAt.Date.ToShortDateString();
            return date;
        }

        //It gets the language of the particular repository (currently being modified)
        public override List<LanguageDetails> GetRepositoryLanguages(string Owner, string RepositoryName)
        {
            List<LanguageDetails> languagesUsed = new List<LanguageDetails>();
            var repository = clientDetail.Repository.Get(Owner, RepositoryName).Result;
            foreach(var language in clientDetail.Repository.GetAllLanguages(repository.Id).Result)
            {
                Console.WriteLine(language.Name+ " "+ language.NumberOfBytes);
                languagesUsed.Add(new LanguageDetails { Name = language.Name, NumberOfBytes = language.NumberOfBytes });
            }
            return languagesUsed;
        }

        //It gets commit author name,commit message, commit date
        public override List<CommitDetailsModel> GetCommitDetails(string Owner, string RepositoryName)
        {
            List<CommitDetailsModel> commitMessages = new List<CommitDetailsModel>();
            var commits = clientDetail.Repository.Commit.GetAll(Owner, RepositoryName).Result;
            foreach (var cm in commits)
            {
                var comit = clientDetail.Repository.Commit.Get(Owner, RepositoryName, cm.Sha).Result;
                commitMessages.Add(new CommitDetailsModel { AuthorName = comit.Commit.Author.Name, CommitMessage = comit.Commit.Message, DateStr = comit.Commit.Committer.Date.Date.ToShortDateString() });
            }
            return commitMessages;
        }

        //It gets total no of commits
        public override int GetTotalNoOfCommits(string Owner, string RepositoryName)
        {
            int totalNoOfCommits = 0;
            var commits = clientDetail.Repository.Commit.GetAll(Owner, RepositoryName).Result;
            foreach (var cm in commits)
            {
                totalNoOfCommits++;
            }
            return totalNoOfCommits;
        }

    }
}
