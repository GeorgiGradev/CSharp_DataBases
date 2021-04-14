using Instagraph.Data;
using Instagraph.DataProcessor.DtoModels.ImportDtos;
using Instagraph.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Instagraph.App.XMLHelper;

namespace Instagraph.DataProcessor
{

    public class Deserializer
    {
        private static string successMessage = "Successfully imported {0}.";
        private static string ErrorMessage = "Error: Invalid data.";

        public static string ImportPictures(InstagraphContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var importPictureDtos = JsonConvert.DeserializeObject<List<ImportPictureDto>>(jsonString);

            var picturesToAdd = new List<Picture>();

            foreach(var importPictureDto in importPictureDtos)
            {
                if (!IsValid(importPictureDto) 
                    || importPictureDto.Size == 0 
                    || String.IsNullOrWhiteSpace(importPictureDto.Path)
                    || importPictureDto.Path.Length == 0
                    || picturesToAdd.Any(x=>x.Path == importPictureDto.Path))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var picture = new Picture()
                {
                    Path = importPictureDto.Path,
                    Size = importPictureDto.Size
                };


                picturesToAdd.Add(picture);
                sb.AppendLine($"Successfully imported Picture {importPictureDto.Path}.");

            }

            context.Pictures.AddRange(picturesToAdd);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportUsers(InstagraphContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var importUserDtos = JsonConvert.DeserializeObject<List<ImportUserDto>>(jsonString);
            var usersToAdd = new List<User>();

            foreach (var importUserDto in importUserDtos)
            {
                if (!IsValid(importUserDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var picture = context.Pictures.FirstOrDefault(x => x.Path == importUserDto.ProfilePicture);

                if (picture == null)
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                var user = new User()
                {
                    ProfilePicture = picture,
                    Password = importUserDto.Password,
                    Username = importUserDto.Username
                };


                usersToAdd.Add(user);

                sb.AppendLine($"Successfully imported User {user.Username}.");


            }
            context.Users.AddRange(usersToAdd);

            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportFollowers(InstagraphContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var importFollowerDtos = JsonConvert.DeserializeObject<List<ImportFollowerDto>>(jsonString);
            var followersToAdd = new List<UserFollower>();

            foreach (var importFollowerDto in importFollowerDtos)
            {
                if (!IsValid(importFollowerDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var user = context.Users.FirstOrDefault(x=>x.Username == importFollowerDto.User);
                var follower = context.Users.FirstOrDefault(x => x.Username == importFollowerDto.Follower);
                var isFollowing = followersToAdd.Any(u => u.User == user && u.Follower == follower);

                if (user == null || follower == null || isFollowing)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
       

                var userFollower = new UserFollower()
                { 
                    User = user,
                    Follower = follower
                };

                followersToAdd.Add(userFollower);
                sb.AppendLine($"Successfully imported Follower {follower.Username} to User {user.Username}.");
            }

            context.UsersFollowers.AddRange(followersToAdd);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportPosts(InstagraphContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var importPostDtos = XMLConverter.Deserializer<ImportPostDto>(xmlString, "posts");
            var postsToAdd = new List<Post>();

            foreach (var importPostDto in importPostDtos)
            {
                if (!IsValid(importPostDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var user = context.Users.FirstOrDefault(x => x.Username == importPostDto.User);
                var picture = context.Pictures.FirstOrDefault(x => x.Path == importPostDto.Picture);

                if (user == null || picture == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var post = new Post()
                {
                    Caption = importPostDto.Caption,
                    User = user,
                    Picture = picture
                };

                postsToAdd.Add(post);
                sb.AppendLine($"Successfully imported Post {post.Caption}.");
            }

            context.AddRange(postsToAdd);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportComments(InstagraphContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var importCommentDtos = XMLConverter.Deserializer<ImportCommnentDto>(xmlString, "comments");
            var commentsToAdd = new List<Comment>();

            foreach (var importCommentDto in importCommentDtos)
            {
                if(!IsValid(importCommentDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var user = context.Users.FirstOrDefault(x => x.Username == importCommentDto.User);
                var post = context.Posts.FirstOrDefault(x => x.Id == importCommentDto.Post.PostId);

                if (user == null || post == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var comment = new Comment()
                {
                    Content = importCommentDto.Content,
                    User = user,
                    Post = post
                };

                commentsToAdd.Add(comment);
                sb.AppendLine($"Successfully imported Comment {comment.Content}.");

            }
            context.Comments.AddRange(commentsToAdd);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
