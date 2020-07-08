//This is a source code or part of Masterfine project
//Copyright (C) 2013  Cybrosys Technologies Pvt.Ltd
//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;    
using System.Collections.Generic;    
using System.Text;    
//<summary>    
//Summary description for UserInfo    
//</summary>    
namespace Masterfine    
{    
class UserInfo    
{    
    private decimal _userId;    
    private string _userName;    
    private string _password;    
    private bool _active;
    private decimal _roleId;
    private string _narration;
    private DateTime _extraDate;    
    private string _extra1;    
    private string _extra2;    
    
    public decimal UserId    
    {    
        get { return _userId; }    
        set { _userId = value; }    
    }    
    public string UserName    
    {    
        get { return _userName; }    
        set { _userName = value; }    
    }    
    public string Password    
    {    
        get { return _password; }    
        set { _password = value; }    
    }    
    public bool Active    
    {    
        get { return _active; }    
        set { _active = value; }    
    }
     public decimal  RoleId   
    {
        get { return _roleId; }
        set { _roleId = value; }    
    }
     public string Narration
     {
         get { return _narration; }
         set { _narration = value; }
     }
    public DateTime ExtraDate    
    {    
        get { return _extraDate; }    
        set { _extraDate = value; }    
    }    
    public string Extra1    
    {
        get { return _extra1; }
        set { _extra1 = value; }    
    }    
    public string Extra2    
    {    
        get { return _extra2; }    
        set { _extra2 = value; }    
    }    
    
}    
}
