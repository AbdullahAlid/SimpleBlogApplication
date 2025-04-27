using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SimpleBlogApplication.DAL.Models;

namespace SimpleBlogApplication.DAL.Filters
{
    public class CheckUserValidityAttribute : TypeFilterAttribute
    {
        public CheckUserValidityAttribute() : base(typeof(CheckUserValidityFilter))
        {
           
        }
    }
}
