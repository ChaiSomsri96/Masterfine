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
//Summary description for MaterialReceiptDetailsInfo    
//</summary>    
namespace Masterfine    
{    
class MaterialReceiptDetailsInfo    
{    
    private decimal _materialReceiptDetailsId;    
    private decimal _materialReceiptMasterId;    
    private decimal _productId;    
    private decimal _orderDetailsId;    
    private decimal _qty;    
    private decimal _rate;    
    private decimal _unitId;    
    private decimal _unitConversionId;    
    private decimal _batchId;    
    private decimal _godownId;    
    private decimal _rackId;    
    private decimal _amount;    
    private int _slno;    
    private DateTime _extraDate;    
    private string _extra1;    
    private string _exta2;
//    private decimal _exchangeRateId;  

    public decimal MaterialReceiptDetailsId    
    {    
        get { return _materialReceiptDetailsId; }    
        set { _materialReceiptDetailsId = value; }    
    }    
    public decimal MaterialReceiptMasterId    
    {    
        get { return _materialReceiptMasterId; }    
        set { _materialReceiptMasterId = value; }    
    }    
    public decimal ProductId    
    {    
        get { return _productId; }    
        set { _productId = value; }    
    }    
    public decimal OrderDetailsId    
    {    
        get { return _orderDetailsId; }    
        set { _orderDetailsId = value; }    
    }    
    public decimal Qty    
    {    
        get { return _qty; }    
        set { _qty = value; }    
    }    
    public decimal Rate    
    {    
        get { return _rate; }    
        set { _rate = value; }    
    }    
    public decimal UnitId    
    {    
        get { return _unitId; }    
        set { _unitId = value; }    
    }    
    public decimal UnitConversionId    
    {    
        get { return _unitConversionId; }    
        set { _unitConversionId = value; }    
    }    
    public decimal BatchId    
    {    
        get { return _batchId; }    
        set { _batchId = value; }    
    }    
    public decimal GodownId    
    {    
        get { return _godownId; }    
        set { _godownId = value; }    
    }    
    public decimal RackId    
    {    
        get { return _rackId; }    
        set { _rackId = value; }    
    }    
    public decimal Amount    
    {    
        get { return _amount; }    
        set { _amount = value; }    
    }    
    public int Slno    
    {    
        get { return _slno; }    
        set { _slno = value; }    
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
    public string Exta2    
    {    
        get { return _exta2; }    
        set { _exta2 = value; }    
    }
    //public decimal ExchangeRateId
    //{
    //    get { return _exchangeRateId; }
    //    set { _exchangeRateId = value; }
    //}    
}    
}
