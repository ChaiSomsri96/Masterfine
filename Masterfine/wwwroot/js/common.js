// Utility functions added by developer 2020.3.18
function ConvertObjectToArray(object) {
    var array = [];
    for (var i in object) {
        if (object.hasOwnProperty(i)) {
            array.push(object[i]);
        }
    }
    return array;
}

function get_date_in_special_format(date) {
    var _date = new Date(date);
    
    var year = _date.getFullYear();
    var month = _date.getMonth() + 1;
    month = month < 10 ? "0" + month : month;
    var day = _date.getDate();
    day = day < 10 ? "0" + day : day;
    var temp = year + "-" + month + "-" + day;

    return temp.toString();
}

function validate_email_format(email) {
    var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;

    if (!emailReg.test(email)) {
        return false;
    }

    return true;
}