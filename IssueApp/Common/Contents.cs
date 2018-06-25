using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IssueApp.Common
{
    public class Contents
    {
        public enum ErrorMessages
        {
            NotAuthenticate
        }

        public static readonly Dictionary<ErrorMessages, string> Messages = new Dictionary<ErrorMessages, string>()
        {
            { ErrorMessages.NotAuthenticate, "ユーザ情報が不正、もしくは存在しません。" }
        };

        public static string GetMessage(ErrorMessages messages)
        {
            return Messages[messages];
        }


    }
}