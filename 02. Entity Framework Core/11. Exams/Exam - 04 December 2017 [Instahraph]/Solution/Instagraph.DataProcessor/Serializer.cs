using Instagraph.App.XMLHelper;
using Instagraph.Data;
using Instagraph.DataProcessor.DtoModels.ExportDtos;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Instagraph.DataProcessor
{
    public class Serializer
    {
        public static string ExportUncommentedPosts(InstagraphContext context)
        {
            var result = context
                .Posts
                .Where(post => post.Comments.Count == 0)
                .Select(post => new
                {
                    Id = post.Id,
                    Picture = post.Picture.Path,
                    User = post.User.Username
                })
                .ToList()
                .OrderBy(post => post.Id)
                .ToList();

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);

            return json;
        }

        public static string ExportPopularUsers(InstagraphContext context)
        {

            var result = context
                .Users
                .Where(user => user.Posts
                    .Any(post => post.Comments
                        .Any(comment => user.Followers
                            .Any(follower => follower.FollowerId == comment.UserId))))
                .OrderBy(user => user.Id)
                .Select(user => new
                {
                    Username = user.Username,
                    Followers = user.Followers.Count()
                })
                .ToList();

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);

            return json;
        }

        public static string ExportCommentsOnPosts(InstagraphContext context)
        {
            var users = context
                            .Users
                            .Select(u => new
                            {
                                Username = u.Username,
                                Comments = u.Posts
                                    .Select(p => p.Comments.Count)
                                    .ToList()
                            });

            var result = new List<ExportUserDto>();

            foreach (var user in users)
            {
                var mostComments = 0;

                if (user.Comments.Any())
                {
                    mostComments = user.Comments
                        .OrderByDescending(c => c)
                        .First();
                }

                var userToAdd = new ExportUserDto()
                {
                    Username = user.Username,
                    MostComments = mostComments
                };

                result.Add(userToAdd);
            }

            var xml = XMLConverter.Serialize(result.OrderByDescending(x=>x.MostComments).ToList(), "users");


            return xml;
        }
    }
}
