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
//Summary description for CurrencyInfo    
//</summary>    
namespace Masterfine    
{    
class CurrencyInfo    
{    
    private decimal _currencyId;    
    private string _currencySymbol;    
    private string _currencyName;    
    private string _subunitName;    
    private int _noOfDecimalPlaces;    
    private string _narration;    
    private bool _isDefault;    
    private DateTime _extraDate;    
    private string _extra1;    
    private string _extra2;    
    
    public decimal CurrencyId    
    {    
        get { return _currencyId; }    
        set { _currencyId = value; }    
    }    
    public string CurrencySymbol    
    {    
        get { return _currencySymbol; }    
        set { _currencySymbol = value; }    
    }    
    public string CurrencyName    
    {    
        get { return _currencyName; }    
        set { _currencyName = value; }    
    }    
    public string SubunitName    
    {    
        get { return _subunitName; }    
        set { _subunitName = value; }    
    }    
    public int NoOfDecimalPlaces    
    {    
        get { return _noOfDecimalPlaces; }    
        set { _noOfDecimalPlaces = value; }    
    }    
    public string Narration    
    {    
        get { return _narration; }    
        set { _narration = value; }    
    }    
    public bool IsDefault    
    {    
        get { return _isDefault; }    
        set { _isDefault = value; }    
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
