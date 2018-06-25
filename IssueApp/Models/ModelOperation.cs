using IssueApp.Models.Json;
using IssueApp.Models.Json.Parts;
using Octokit;
using System.Collections.Generic;
using static IssueApp.Models.Json.Dialog;
using static IssueApp.Models.Json.PostMessageModel;
using static IssueApp.Models.Json.PostMessageModel.Attachment;

namespace IssueApp.Models
{
    public static class ModelOperation
    {
        #region Issue作成用ダイアログモデル作成
        /// <summary>
        /// Issue作成用ダイアログモデル作成
        /// </summary>
        /// <param name="trigger_id"></param>
        /// <param name="labelNameList"></param>
        /// <returns></returns>
        public static DialogModel CreateDialogModelForCreateIssue(string trigger_id, List<string> labelNameList)
        {
            return new DialogModel()
            {
                Trigger_id = trigger_id,
                Dialog = new Dialog()
                {
                    Callback_id = "createissue",
                    Title = "Issue登録",
                    Submit_label = "登録",
                    Elements = new List<Element>
                        {
                            new Element()
                            {
                                Type = "select",
                                Label = "ラベル",
                                Name = "label",
                                Options = labelNameList.ConvertAll(x => new Element.Option()
                                {
                                    Label = x,
                                    Value = x
                                })
                            },
                            new Element()
                            {
                                Type = "text",
                                Label = "タイトル",
                                Name = "title"
                            },
                            new Element()
                            {
                                Type = "textarea",
                                Label = "本文",
                                Name = "body"
                            }
                        }
                }
            };
        }
        #endregion

        #region リポジトリ登録用ダイアログモデル作成
        /// <summary>
        /// リポジトリ登録用ダイアログモデル作成
        /// </summary>
        /// <param name="trigger_id"></param>
        /// <returns></returns>
        public static DialogModel CreateDialogModelForSetRespotory(string trigger_id)
        {
            return new DialogModel()
            {
                Trigger_id = trigger_id,
                Dialog = new Dialog()
                {
                    Callback_id = "setrepository",
                    Title = "リポジトリ登録",
                    Submit_label = "登録",
                    Elements = new List<Element>
                    {
                        new Element()
                        {
                            Type = "text",
                            Label = "ユーザ名",
                            Name = "username"
                        },
                        new Element()
                        {
                            Type = "text",
                            Label = "リポジトリ名",
                            Name = "repository"
                        }
                    }
                }
            };
        }
        #endregion

        #region Issue閲覧用メッセージ表示モデル作成
        /// <summary>
        /// Issue閲覧用メッセージ表示モデル作成
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="issueList"></param>
        /// <returns></returns>
        public static PostMessageModel CreatePostMessageModelForDisplayIssue(string channelId, List<Issue> issueList)
        {
            return new PostMessageModel()
            {
                Channel = channelId,
                Text = "Issueの一覧を照会します。",
                Attachments = new List<Attachment>
                {
                    new Attachment()
                    {
                        Callback_id = "displayissue",
                        Fallback = "The Display Issue",
                        Attachment_type = "default",
                        Actions = new List<Action>
                        {
                            new Action()
                            {
                                Type = "select",
                                Name = "Issue一覧",
                                Text = "Issue一覧",
                                Options = issueList.ConvertAll(x => new Option()
                                {
                                    Text = x.Title,
                                    Value = x.Number.ToString()
                                })
                            }
                        }
                    }
                }
            };
        }
        #endregion
    }
}