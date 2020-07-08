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
//Summary description for BonusDedutionInfo    
//</summary>    
namespace Masterfine    
{    
class BonusDedutionInfo    
{    
    private decimal _bonusDeductionId;    
    private decimal _employeeId;    
    private DateTime _date;    
    private DateTime _month;    
    private decimal _bonusAmount;    
    private decimal _deductionAmount;    
    private string _narration;    
    private DateTime _extraDate;    
    private string _extra1;    
    private string _extra2;    
    
    public decimal BonusDeductionId    
    {    
        get { return _bonusDeductionId; }    
        set { _bonusDeductionId = value; }    
    }    
    public decimal EmployeeId    
    {    
        get { return _employeeId; }    
        set { _employeeId = value; }    
    }    
    public DateTime Date    
    {    
        get { return _date; }    
        set { _date = value; }    
    }    
    public DateTime Month    
    {    
        get { return _month; }    
        set { _month = value; }    
    }    
    public decimal BonusAmount    
    {    
        get { return _bonusAmount; }    
        set { _bonusAmount = value; }    
    }    
    public decimal DeductionAmount    
    {    
        get { return _deductionAmount; }    
        set { _deductionAmount = value; }    
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
