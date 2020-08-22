$(function () {
    // ReSharper disable once PossiblyUnassignedProperty
    $("#customer-grid").kendoGrid({
        dataSource: {
            transport: {
                read: {
                    url: "/Customer/KendouiCustomerList",
                    type: "POST",
                    contentType: "application/json",
                    dataType: "json",
                    data: additionalData
                },
                create: {
                    url: "/Customer/KendoCreateCustomer",
                    type: "POST",
                    dataType: "json"
                },
                update: {
                    url: "/Customer/KendoEditCustomer",
                    type: "POST",
                    dataType: "json"
                },
                destroy: {
                    url: "/Customer/KendoDeleteCustomer",
                    type: "POST",
                    dataType: "json"
                },
                parameterMap: function(data, operation) {
                    if (operation !== "read") {
                        return data;
                    } else {
                        var postData = {
                            Username: data.Username,
                            Email: data.Email,
                            PageIndex: data.page,
                            PageSize: data.pageSize
                        };
                        // ReSharper disable once UseOfImplicitGlobalInFunctionScope
                        return JSON.stringify(postData);

                    }
                }
            },
            schema: {
                data: "Data",
                total: "Total",
                errors: "Errors",
                model: {
                    id: "Id",
                    fields: {
                        Username: {
                            editable: true,
                            type: "string",
                            validation: {
                                required: {
                                    message: "Please input the Username."
                                }
                            }
                        },
                        Email: {
                            editable: true,
                            type: "string"
                        },
                        Active: {
                            editable: true,
                            type: "boolean"
                        },
                        CreationTime: {
                            editable: false,
                            type: "string"
                        },
                        Id: { editable: false, type: "number" }
                    }
                }
            },
            requestEnd: function(e) {
                if (e.type === "create" || e.type === "update") {
                    this.read();
                }
            },
            error: function(e) {
                if (e.errors) {
                    if ((typeof e.errors) === "string") {
                        //single error
                        //display the message
                        $.alert("Error happened:\n" + e.errors);
                    }
                }
                // Cancel the changes
                this.cancelChanges();
            },
            pageSize: 10,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true
        },
        pageable: {
            buttonCount: 3,
            input: true,
            pageSizes: [5, 10, 15, 20, 25, 30, 50, 100],
            refresh: true
        },
        toolbar: [{ name: "create", text: "Add" }],
        editable: {
            confirmation: "Are you sure to delete this record ?",
            mode: "popup"
        },
        scrollable: true,
        columns: [
            {
                field: "Username",
                title: "Name",
                width: 300
            }, {
                field: "Email",
                title: "Email",
                editor: function(container, options) {
                    $('<input class="k-textbox" name="Email" type="email" data-bind="value: ' +
                            options.field +
                            '" data-email-msg="The email format is incorrect." required>')
                        .appendTo(container);
                },
                width: 300
            }, {
                field: "Active",
                title: "Active",
                headerAttributes: { style: "text-align:center" },
                attributes: { style: "text-align:center" },
                template:
                    '# if(Active) {# <span class="k-icon k-i-checkmark"></span> #} else {# <span class="k-icon k-i-close"></span> #} #',
                width: 100
            }, {
                field: "CreationTime",
                title: "CreationTime",
                attributes: { style: "text-align:center" },
                width: 150
            }, {
                command: [
                    {
                        name: "edit",
                        text: {
                            edit: "Edit",
                            update: "Update",
                            cancel: "Cancel"
                        }
                    }, {
                        name: "destroy",
                        text: "Delete"
                    }
                ],
                width: 200,
                title: "Action"
            }
        ],
        edit: function (e) {
            e.container.find("div.k-edit-label:last").hide();
            e.container.find("div.k-no-editor:last").hide();
        }
    });

    $("#btn-search").click(function () {
        var grid = $("#customer-grid").data("kendoGrid");
        grid.dataSource.page(1);
        return false;
    });
});

function additionalData() {
    var data = {
        Username: $("#search-username").val(),
        Email: $("#search-email").val()
    };

    return data;
}
$("".concat("#search-username,", "#search-email")).keydown(function (event) {
    if (event.keyCode === 13) {
        $("#btn-search").click();
        return false;
    }
    // ReSharper disable once NotAllPathsReturnValue
});