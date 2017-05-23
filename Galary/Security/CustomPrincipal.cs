using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Galary.Security
{
    public class CustomPrincipal: IPrincipal
    {
        public CustomPrincipal(string userName)
        {
            this.Identity = new GenericIdentity(userName);
        }

        public IIdentity Identity
        {
            get;
            private set;
        }

        public int CurrentUserID { get; set; }

        public string CurrentUserName { get; set; }

        public List<int> CurrentRoles { get; set; }

        public bool IsInRole(string role)
        {
            return CurrentRoles.Contains(Convert.ToInt32(role));
        }
    }

    public class CustomPrincipalSerializeModel
    {
        public int CurrentUserID { get; set; }

        public string CurrentUserName { get; set; }

        public List<int> CurrentRoles { get; set; }
    }
}