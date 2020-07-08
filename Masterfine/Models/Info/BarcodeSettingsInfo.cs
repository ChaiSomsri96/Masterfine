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
//Summary description for BarcodeSettingsInfo    
//</summary>    
namespace Masterfine    
{    
class BarcodeSettingsInfo    
{    
    private decimal _barcodeSettingsId;    
    private bool _showCompanyName;
    private bool _showProductCode;
    private string _companyName;    
    private bool _showPurchaseRate;    
    private bool _showMRP;    
    private string _point;    
    private string _zero;    
    private string _one;    
    private string _two;    
    private string _three;    
    private string _four;    
    private string _five;    
    private string _six;    
    private string _seven;    
    private string _eight;    
    private string _nine;    
    private string _extra1;    
    private string _extra2;    
    private DateTime _extraDate;    
    
    public decimal BarcodeSettingsId    
    {    
        get { return _barcodeSettingsId; }    
        set { _barcodeSettingsId = value; }    
    }    
    public bool ShowCompanyName    
    {    
        get { return _showCompanyName; }    
        set { _showCompanyName = value; }    
    }

    public bool ShowProductCode
    {
        get { return _showProductCode; }
        set { _showProductCode = value; }
    } 
    public string CompanyName    
    {    
        get { return _companyName; }    
        set { _companyName = value; }    
    }    
    public bool ShowPurchaseRate    
    {    
        get { return _showPurchaseRate; }    
        set { _showPurchaseRate = value; }    
    }    
    public bool ShowMRP    
    {    
        get { return _showMRP; }    
        set { _showMRP = value; }    
    }    
    public string Point    
    {    
        get { return _point; }    
        set { _point = value; }    
    }    
    public string Zero    
    {    
        get { return _zero; }    
        set { _zero = value; }    
    }    
    public string One    
    {    
        get { return _one; }    
        set { _one = value; }    
    }    
    public string Two    
    {    
        get { return _two; }    
        set { _two = value; }    
    }    
    public string Three    
    {    
        get { return _three; }    
        set { _three = value; }    
    }    
    public string Four    
    {    
        get { return _four; }    
        set { _four = value; }    
    }    
    public string Five    
    {    
        get { return _five; }    
        set { _five = value; }    
    }    
    public string Six    
    {    
        get { return _six; }    
        set { _six = value; }    
    }    
    public string Seven    
    {    
        get { return _seven; }    
        set { _seven = value; }    
    }    
    public string Eight    
    {    
        get { return _eight; }    
        set { _eight = value; }    
    }    
    public string Nine    
    {    
        get { return _nine; }    
        set { _nine = value; }    
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
    public DateTime ExtraDate    
    {    
        get { return _extraDate; }    
        set { _extraDate = value; }    
    }    
    
}    
}
