using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCGrid.Models;
using MVCGrid.Web;
using MyEverNote.BusinessLayer;
using MyEverNote.Entities;

namespace MyEverNote.WebApp.App_Start
{
    public class MVCGridConfig
    {
        public static void RegisterGrids()
        {
            MVCGridDefinitionTable.Add("NoteGrid", new MVCGridBuilder<Note>()
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .WithSorting(sorting: true, defaultSortColumn: "Id", defaultSortDirection: SortDirection.Dsc)
                .AddColumns(cols =>
                {
                    cols.Add("Id").WithValueExpression(p => p.Id.ToString());
                    cols.Add("Title").WithHeaderText("Başlık").WithValueExpression(p => p.Title);
                    cols.Add("Category.Title").WithHeaderText("Kategori").WithValueExpression(p => p.Category.Title);
                    cols.Add("EverNoteUser.Name " + "p.EverNoteUser.Surname").WithHeaderText("Yazar").WithValueExpression(p => p.EverNoteUser.Name + " " + p.EverNoteUser.Surname);
                })
                .WithRetrieveDataMethod((context) =>
                {
                    var options = context.QueryOptions;
                    var result = new QueryResult<Note>();

                    Manager<Note> NoteManager = new Manager<Note>();
                    List<Note> Notes = NoteManager.GetAll().ToList();
                    if (!String.IsNullOrWhiteSpace(options.SortColumnName))
                    {
                        switch (options.SortColumnName.ToLower())
                        {
                            case "title":
                                if (options.SortDirection == SortDirection.Asc)
                                    Notes = Notes.OrderBy(c => c.Title).ToList();
                                else if (options.SortDirection == SortDirection.Dsc)
                                    Notes = Notes.OrderByDescending(c => c.Title).ToList();

                                //var a = options.SortDirection == SortDirection.Asc ? Notes.OrderBy(p => p.Title) : Notes.OrderByDescending(p => p.Title);
                                //Notes = Notes.OrderBy(p=>p.Title,a);
                                //Notes = Notes.OrderBy(p => p.Title,options.SortDirection).ToList();
                                break;
                            case "category.title":
                                if (options.SortDirection == SortDirection.Asc)
                                    Notes = Notes.OrderBy(c => c.Category.Title).ToList();
                                else if (options.SortDirection == SortDirection.Dsc)
                                    Notes = Notes.OrderByDescending(c => c.Category.Title).ToList();
                                break;
                            case "evernoteuser.name " + "evernoteuser.surname":
                                if (options.SortDirection == SortDirection.Asc)
                                    Notes = Notes.OrderBy(c => c.EverNoteUser.Name).ToList();
                                else if (options.SortDirection == SortDirection.Dsc)
                                    Notes = Notes.OrderByDescending(c => c.EverNoteUser.Name).ToList();
                                break;
                        }
                    }

                    result.Items = Notes;

                    return result;
                })
            );
        }
    }
}