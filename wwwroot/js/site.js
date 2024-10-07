// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.

if (!Array.prototype.sortByObjectProperty) {
    Array.prototype.sortByObjectProperty = function (propName, descending) {
        return this.sort(function (a, b) {
            if (typeof b[propName] === 'number' && typeof a[propName] === 'number') {
                return (descending) ? b[propName] - a[propName] : a[propName] - b[propName];
            } else if (typeof b[propName] === 'string' && typeof a[propName] === 'string') {
                return (descending) ? b[propName] > a[propName] : a[propName] > b[propName];
            } else {
                return this;
            }
        });
    };
}

if (jQuery) {
    jQuery.expr.filters.visible = function (elem) {
        return !!(elem.offsetWidth || elem.offsetHeight || elem.getClientRects().length);
    };
}

if (jQuery) {
    jQuery.expr.filters.visible = function (elem) {
        return !!(elem.offsetWidth || elem.offsetHeight || elem.getClientRects().length);
    };
}

function addEventToPGTable(contactId, eventType, eventId) {
    try {
        jQuery.ajax({
            type: "POST",
            url: "/EventMonitor?handler=GetAjax",
            data: { "Contactid": contactId, "Eventtype": eventType, "EventId": eventId },
            //data: { "FirstName": "Ecommerce", "LastName": "Demo", "Email": "customer@geniedemos.com", "EventType": eventType, "EventId": eventId },
            contentType: 'application/x-www-form-urlencoded',
            dataType: "json",
            async: false,
            headers:
            {
                "RequestVerificationToken": $('input:hidden[name="__RequestVerificationToken"]').val()
            },
            beforeSend: function () {
                jQuery("#divMask").show();
            },
            success: function (msg) {
                jQuery("#divMask").hide();
                console.log(msg);
            },
            error: function (error) {
                jQuery("#divMask").hide();
                console.log(error);
            }
        });
    }
    catch (err) {
        console.error(err);
    }
}

function initEventMonitorDataTable(timezone) {
    if ($("#dcevents").length > 0) {
        if ($("#dcevents").dataTable) {
            $("#dcevents").dataTable().fnDestroy();
        }

        $('#dcevents').DataTable({
            searching: false, paging: false, info: false,
            columnDefs: [
                {
                    render: function (data, type, row) {
                        //return data;
                        if (/<\/?[a-z][\s\S]*>/i.test(data)) {
                            if ($(data).text().trim() == '')
                                return data;
                        }
                        else if (data.trim() == '') {
                            return data;
                        }
                        else if (!timezone) {
                            return data;
                        }

                        if (jQuery(data).attr("data-date")) {
                            let utctime = moment.utc(jQuery(data).attr("data-date"), 'DD-MM-YYYY HH:mm:ss A');
                            return jQuery(data).html(utctime.tz(timezone).format("DD-MM-YYYY HH:mm:ss A")).prop('outerHTML');
                        }
                        else {
                            return data;
                        }
                    },
                    targets: 2,
                },
                {
                    render: function (data, type, row, meta) {
                        //return data;
                        if (/<\/?[a-z][\s\S]*>/i.test(data)) {
                            if ($(data).text().trim() == '')
                                return data;
                        }
                        else if (data.trim() == '') {
                            return data;
                        }
                        else if (!timezone) {
                            return data;
                        }

                        if (jQuery(data).attr("data-date")) {
                            let utctime = moment.utc(jQuery(data).attr("data-date"), 'YYYY-MM-DD HH:mm:ss A');
                            return jQuery(data).html(utctime.tz(timezone).format("DD-MM-YYYY HH:mm:ss A")).prop('outerHTML');
                        }
                        else {
                            return data;
                        }
                    },
                    targets: 3,
                }
            ],
        });
    }
	
	if ($("#webhook").length > 0) {
    if ($("#webhook").dataTable) {
        $("#webhook").dataTable().fnDestroy();
    }

    $('#webhook').DataTable({
        searching: false, paging: false, info: false,
        columnDefs: [
            {
                render: function (data, type, row) {
                    //return data;
                    if (/<\/?[a-z][\s\S]*>/i.test(data)) {
                        if ($(data).text().trim() == '')
                            return data;
                    }
                    else if (data.trim() == '') {
                        return data;
                    }
                    else if (!timezone) {
                        return data;
                    }

                    if (jQuery(data).attr("data-date")) {
                        let utctime = moment.utc(jQuery(data).attr("data-date"), 'DD-MM-YYYY HH:mm:ss A');
                        return jQuery(data).html(utctime.tz(timezone).format("DD-MM-YYYY HH:mm:ss A")).prop('outerHTML');
                    }
                    else {
                        return data;
                    }
                },
                targets: 2,
            },
            {
                render: function (data, type, row, meta) {
                    //return data;
                    if (/<\/?[a-z][\s\S]*>/i.test(data)) {
                        if ($(data).text().trim() == '')
                            return data;
                    }
                    else if (data.trim() == '') {
                        return data;
                    }
                    else if (!timezone) {
                        return data;
                    }

                    if (jQuery(data).attr("data-date")) {
                        let utctime = moment.utc(jQuery(data).attr("data-date"), 'DD-MM-YYYY HH:mm:ss A');
                        return jQuery(data).html(utctime.tz(timezone).format("DD-MM-YYYY HH:mm:ss A")).prop('outerHTML');
                    }
                    else {
                        return data;
                    }
                },
                targets: 3,
            },
            {
                render: function (data, type, row, meta) {
                    //return data;
                    if (/<\/?[a-z][\s\S]*>/i.test(data)) {
                        if ($(data).text().trim() == '')
                            return data;
                    }
                    else if (data.trim() == '') {
                        return data;
                    }
                    else if (!timezone) {
                        return data;
                    }

                    if (jQuery(data).attr("data-date")) {
                        let utctime = moment.utc(jQuery(data).attr("data-date"), 'DD-MM-YYYY HH:mm:ss A');
                        return jQuery(data).html(utctime.tz(timezone).format("DD-MM-YYYY HH:mm:ss A")).prop('outerHTML');
                    }
                    else {
                        return data;
                    }
                },
                targets: 4,
            }
        ],
    });
}
}
