using IssueApp.Models.Json;
using System.Collections.Generic;
using static IssueApp.Models.Json.Dialog;
using static IssueApp.Models.Json.Dialog.Element;

namespace IssueApp.Models
{
    public static class ModelOperation
    {
        #region Issue作成ダイアログ
        /// <summary>
        /// Issue作成ダイアログ
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
                                Options = labelNameList.ConvertAll(x => new Option(x, x))

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
    }
}