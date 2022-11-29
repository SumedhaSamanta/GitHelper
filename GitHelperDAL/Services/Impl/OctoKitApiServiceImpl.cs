/* 
 Created By:        Shubham Jaiswal
 Created Date:      23-10-2022
 Modified Date:     17-11-2022
 Purpose:           This is the implementation class of GitHubApiService class.This class uses Octokit to communicate
                    with  GitHub Api.
 Purpose Type:      This class holds the implementation of all the abstract method of the GitHubApiService class.
 Referenced files:  Model/CommitDetailsModel.cs, Model/LanguageDetails.cs, Model/ParticularRepoDetailsModel.cs, Model/RepoDetailModel.cs, Model/RepoInfoModel.cs, Response/UserDetailsResponse.cs
 */
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

        /*
            <summary>
            It authenticates user with userName and Personal Access Token    
            </summary>
            <param> NA </param>
            <returns>Returns true or false</returns>
        */
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

        /*
            <summary>
            It gets all the basic information of the use    
            </summary>
            <param> NA </param>
            <returns>Returns all the basic details of the authenticated user.</returns>
        */
        public static Task<User> GetUser(GitHubClient client)
        {
            var user = client.User.Current();
            return user;
        }


        /*
           <summary>
            It gets the url of the GitHub User Avatar    
           </summary>
           <param> NA </param>
           <returns>Returns the url of the GitHub Avatar of the user.</returns>
       */
        public override string GetAvtarUrl()
        {
            var task = GetUser(clientDetail);
            User user = task.Result;

            return (user.AvatarUrl);

        }

        /*
           <summary>
            It gets all the basic details of the user    
           </summary>
           <param> NA </param>
           <returns>Returns the user_id,username and url of the GitHub Avatar of the user.</returns>
       */
        public override UserModel GetUserDetails()
        {
            var task = GetUser(clientDetail);
            User user = task.Result;
            return new UserModel { userId = user.Id, userName = user.Login, userAvatarUrl = user.AvatarUrl};
        }

        /*
           <summary>
            It gets all the repositories of the user    
           </summary>
           <param> NA </param>
           <returns>Returns the repo_id,name and owner name of all the repositories.</returns>
       */
        public override List<RepositoryDetailsModel> GetRepositoryDetails()
        {
            var repositories = clientDetail.Repository.GetAllForCurrent().Result;
            List<RepositoryDetailsModel> repoList = new List<RepositoryDetailsModel>();
            foreach (var repository in repositories)
            {
                repoList.Add(new RepositoryDetailsModel { repoId = repository.Id,repoName = repository.Name,repoOwner = repository.Owner.Login});
            }
            return repoList;
        }


/*
   <summary>
    It gets the list of repository name and owner.    
   </summary>
   <param> NA </param>
   <returns>Returns the list of the repository name and owner name.</returns>
*/
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

       
        /*
           <summary>
            It gets the details of the specified repository.    
           </summary>
           <param name="owner"> This is name of the owner of the repository. </param>
           <param name="repositoryName"> This is name of the repository. </param>
           <returns>Returns all the details of the specified repository.</returns>
       */
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

        
        /*
          <summary>
           Returns creation date time of the specified repository.    
          </summary>
          <param name="owner"> This is name of the owner of the repository. </param>
          <param name="repositoryName"> This is name of the repository. </param>
          <returns>Creation date time.</returns>
      */
        public override DateTimeOffset GetRepositoryCreationDate(string owner, string repositoryName)
        {
            var repository = clientDetail.Repository.Get(owner, repositoryName).Result;
            DateTimeOffset date = repository.CreatedAt.UtcDateTime;
            return date;
        }

       
        /*
          <summary>
           It gets the language of specified repository .   
          </summary>
          <param name="owner"> This is name of the owner of the repository. </param>
          <param name="repositoryName"> This is name of the repository. </param>
          <returns>List of languages.</returns>
      */
        public override List<LanguageDetails> GetRepositoryLanguages(string owner, string repositoryName)
        {
            List<LanguageDetails> languagesUsed = new List<LanguageDetails>();
            foreach(var language in clientDetail.Repository.GetAllLanguages(owner, repositoryName).Result)
            {
                languagesUsed.Add(new LanguageDetails { language = language.Name, bytesOfCode = language.NumberOfBytes });
            }
            return languagesUsed;
        }

        
        /*
          <summary>
           It gets commit author name,commit message, commit date.    
          </summary>
          <param name="owner"> This is name of the owner of the repository. </param>
          <param name="repositoryName"> This is name of the repository. </param>
          <returns>List of commit details.</returns>
      */
        public override List<CommitDetailsModel> GetCommitDetails(string owner, string repositoryName)
        {
            List<CommitDetailsModel> commitMessages = new List<CommitDetailsModel>();
            try
            {

                var commits = clientDetail.Repository.Commit.GetAll(owner, repositoryName).Result;
                foreach (var commit in commits)
                {
                    commitMessages.Add(new CommitDetailsModel { commitAuthorName = commit.Commit.Author.Name, commitMessage = commit.Commit.Message, commitDateTime = commit.Commit.Committer.Date }) ;
                }
                return commitMessages;
            }
            catch (AggregateException ex)
            {
                ex.Handle(x =>
                {
                    if (x is Octokit.ApiException)
                    {
                        if (x.Message == "Git Repository is empty.")
                        {
                            return true;
                        }
                    }
                    // Other exceptions will not be handled here.
                    return false;

                });
                return commitMessages;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        /*
          <summary>
           It gets total no of commits of specified repository.    
          </summary>
          <param name="owner"> This is name of the owner of the repository. </param>
          <param name="repositoryName"> This is name of the repository. </param>
          <returns>Number of commits.</returns>
      */
        public override int GetTotalNoOfCommits(string owner, string repositoryName)
        { 
            var commits = clientDetail.Repository.Commit.GetAll(owner, repositoryName).Result;
            
            return commits.Count;
        }

        
        /*
          <summary>
           It gets all the commits made within a time interval for a given repository.    
          </summary>
          <param name="owner"> This is name of the owner of the repository. </param>
          <param name="repositoryName"> This is name of the repository. </param>
          <param name="startTimeStamp"> This is starting date time. </param>
          <param name="endTimeStamp"> This is the ending date time. </param>
          <returns>List of commits for the specified time interval.</returns>
      */
        public override List<CommitDetailsModel> GetCommitsForInterval(string owner, string repositoryName, DateTimeOffset startTimeStamp, DateTimeOffset endTimeStamp)
        {

            List<CommitDetailsModel> commitMessages = new List<CommitDetailsModel>();
            try
            {
                var request = new CommitRequest { Since = startTimeStamp, Until = endTimeStamp };
                var commitList = clientDetail.Repository.Commit.GetAll(owner, repositoryName, request).Result;
                foreach (var commit in commitList)
                {
                    commitMessages.Add(new CommitDetailsModel { commitAuthorName = commit.Commit.Author.Name, commitMessage = commit.Commit.Message, commitDateTime = commit.Commit.Committer.Date });
                }
                return commitMessages;
            }
            catch (AggregateException ex)
            {
                ex.Handle(x =>
                {
                    if (x is Octokit.ApiException)
                    {
                        if (x.Message == "Git Repository is empty.")
                        {
                            return true;
                        }
                    }
                    // Other exceptions will not be handled here.
                    return false;

                });
                return commitMessages;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }


        /*
          <summary>
           Get the specified page of commits for the specified repository. This api requires pageNumber and pageSize to work.    
          </summary>
          <param name="owner"> This is name of the owner of the repository. </param>
          <param name="repositoryName"> This is name of the repository. </param>
          <param name="pageNumber"> This is page number. </param>
          <param name="pageSize"> This is the page size. </param>
          <returns>List of commits.</returns>
      */
        public override List<CommitDetailsModel> GetPaginatedCommits(string owner, string repositoryName, int pageNumber, int pageSize)
        {
            List<CommitDetailsModel> commitMessages = new List<CommitDetailsModel>();
            try
            {
                
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
            catch(AggregateException ex)
            {
                ex.Handle(x =>
                {
                    if (x is Octokit.ApiException)
                    {
                       if(x.Message == "Git Repository is empty.")
                        {
                            return true;
                        }                       
                    }
                    // Other exceptions will not be handled here.
                    return false;

                });
                return commitMessages;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}
