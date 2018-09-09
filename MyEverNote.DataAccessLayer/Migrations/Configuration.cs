//namespace MyEverNote.DataAccessLayer.Migrations
//{
//    using MyEverNote.Entities;
//    using System;
//    using System.Collections.Generic;
//    using System.Data.Entity;
//    using System.Data.Entity.Migrations;
//    using System.Linq;

//    internal sealed class Configuration : DbMigrationsConfiguration<MyEverNote.DataAccessLayer.EF.DatabaseContext>
//    {
//        public Configuration()
//        {
//            AutomaticMigrationsEnabled = true;
//            AutomaticMigrationDataLossAllowed = true;
//        }

//        protected override void Seed(MyEverNote.DataAccessLayer.EF.DatabaseContext context)
//        {
//            {
//                EverNoteUser admin = new EverNoteUser()
//                {
//                    ActivateGuid = Guid.NewGuid(),
//                    CreatenOn = DateTime.Now,
//                    Email = "akdenizberkay@gmail.com",
//                    IsActive = true,
//                    IsAdmin = true,
//                    ModifiedOn = DateTime.Now.AddMinutes(5),
//                    ModifiedUser = "akdenizberkay",
//                    Name = "Berkay",
//                    Password = "123",
//                    Surname = "Akdeniz",
//                    Username = "akdenizberkay"
//                };

//                EverNoteUser standartuser = new EverNoteUser()
//                {
//                    ActivateGuid = Guid.NewGuid(),
//                    CreatenOn = DateTime.Now,
//                    Email = "akdenizdeniz@gmail.com",
//                    IsActive = true,
//                    IsAdmin = false,
//                    ModifiedOn = DateTime.Now.AddMinutes(5),
//                    ModifiedUser = "akdenizdeniz",
//                    Name = "Deniz",
//                    Password = "123",
//                    Surname = "Akdeniz",
//                    Username = "akdenizdeniz"
//                };

//                context.EverNoteUsers.Add(admin);
//                context.EverNoteUsers.Add(standartuser);

//                for (int i = 0; i < 8; i++)
//                {
//                    EverNoteUser user = new EverNoteUser();
//                    user.Name = FakeData.NameData.GetFirstName();
//                    user.Surname = FakeData.NameData.GetSurname();
//                    user.Email = FakeData.NetworkData.GetEmail(user.Name, user.Surname);
//                    user.IsActive = true;
//                    user.ActivateGuid = Guid.NewGuid();
//                    user.IsAdmin = false;
//                    user.Username = user.Surname + user.Name;
//                    user.Password = "123";
//                    user.CreatenOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now);
//                    user.ModifiedOn = FakeData.DateTimeData.GetDatetime(user.CreatenOn, DateTime.Now);
//                    user.ModifiedUser = user.Username;

//                    context.EverNoteUsers.Add(user);
//                }

//                context.SaveChanges();

//                List<EverNoteUser> users = context.EverNoteUsers.ToList();

//                for (int i = 0; i < 10; i++)
//                {
//                    Category cat = new Category();

//                    cat.Title = FakeData.PlaceData.GetStreetName();
//                    cat.Description = FakeData.PlaceData.GetAddress();
//                    cat.CreatenOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now);
//                    cat.ModifiedOn = FakeData.DateTimeData.GetDatetime(cat.CreatenOn, DateTime.Now);
//                    cat.ModifiedUser = "akdenizberkay";

//                    context.Categories.Add(cat);

//                    for (int k = 0; k < FakeData.NumberData.GetNumber(5, 10); k++)
//                    {
//                        Note n = new Note();
//                        n.Title = FakeData.TextData.GetAlphabetical(FakeData.NumberData.GetNumber(5, 20));
//                        n.Text = FakeData.TextData.GetSentences(FakeData.NumberData.GetNumber(5, 10));
//                        //n.Category = cat;
//                        n.IsDraft = false;
//                        n.LikeCount = FakeData.NumberData.GetNumber(1, 10);

//                        Random r = new Random();
//                        n.Owner = users[r.Next(10)];
//                        n.CreatenOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now);
//                        n.ModifiedOn = FakeData.DateTimeData.GetDatetime(n.CreatenOn, DateTime.Now);
//                        n.ModifiedUser = n.Owner.Name;
//                        cat.Notes.Add(n);

//                        for (int j = 0; j < FakeData.NumberData.GetNumber(5, 10); j++)
//                        {
//                            Comment com = new Comment();
//                            com.Text = FakeData.TextData.GetSentence();
//                            Random r2 = new Random();
//                            com.CommentOwner = users[r2.Next(10)];
//                            com.CreatenOn = FakeData.DateTimeData.GetDatetime(n.CreatenOn, DateTime.Now);
//                            com.ModifiedOn = FakeData.DateTimeData.GetDatetime(com.CreatenOn, DateTime.Now);
//                            com.ModifiedUser = com.CommentOwner.Name;
//                            //com.Note = n;
//                            n.Comments.Add(com);
//                        }



//                        for (int l = 0; l < n.LikeCount; l++)
//                        {
//                            Liked like = new Liked();
//                            //like.Note = n;
//                            Random r3 = new Random();
//                            EverNoteUser lu = users[r3.Next(10)];

//                            if (n.Likes.Contains(n.Likes.Where(p => p.User.Name == lu.Name).FirstOrDefault()) == false)
//                            {
//                                like.User = lu;
//                                n.Likes.Add(like);
//                            }
//                            else
//                            {
//                                bool mevcut = false;

//                                while (mevcut == false)
//                                {
//                                    Random r2 = new Random();
//                                    EverNoteUser lu2 = users[r2.Next(10)];
//                                    //if (n.Likes.Count == 0)
//                                    //{
//                                    //    like.User = lu2;
//                                    //    n.Likes.Add(like);
//                                    //    mevcut = true;
//                                    //}
//                                    //else
//                                    //{
//                                    while (n.Likes.Contains(n.Likes.Where(p => p.User.Name == lu2.Name).FirstOrDefault()) == false)
//                                    {
//                                        mevcut = true;
//                                        like.User = lu2;
//                                        n.Likes.Add(like);
//                                    }
//                                    //}
//                                }
//                            }
//                        }
//                    }

//                    context.SaveChanges();

//                }
//            }
//            //  This method will be called after migrating to the latest version.

//            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
//            //  to avoid creating duplicate seed data.
//        }

//    }
//}
