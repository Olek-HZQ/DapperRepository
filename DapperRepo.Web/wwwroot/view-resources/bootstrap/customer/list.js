(function ($) {
    var table = $("#customer-table"),
        createModal = $("#CustomerCreateModal"),
        form = createModal.find("form");

    table.bootstrapTable({
        url: "/Customer/BootstrapCustomerList",
        method: "POST",
        contentType: "application/x-www-form-urlencoded",
        cache: false,
        pagination: true,
        queryParams: function (e) {
            var param = {
                pageIndex: (e.offset / e.limit) + 1,
                pageSize: e.limit,
                Username: $.trim($("#search-username").val()),
                Email: $.trim($("#search-email").val())
            };
            return param;
        },
        sidePagination: "server",
        pageNumber: 1,
        pageSize: 10,
        uniqueId: "Id",
        columns: [
            {
                field: "Id",
                title: "ID",
                visible: false
            }, {
                field: "Username",
                title: "Username"
            }, {
                field: "Email",
                title: "Email"
            }, {
                field: "Active",
                title: "Active",
                align: "center",
                formatter: function (e) {
                    if (e) {
                        return "<i class=\"fa fa-check\" aria-hidden=\"true\"></i>";
                    } else {
                        return "<i class=\"fa fa-times\" aria-hidden=\"true\"></i>";
                    }
                }
            }, {
                field: "CreationTime",
                title: "CreationTime",
                align: "center"
            },
            {
                field: "Action",
                title: "Action",
                align: "center",
                formatter: (value, row) => {
                    var str = "";
                    str += "<button type=\"button\" class=\"btn btn-outline-primary btn-sm edit-customer\" data-customer-id=" +
                        row.Id +
                        " data-toggle=\"modal\" data-target=\"#CustomerEditModal\"><i class=\"fa fa-edit\" aria-hidden=\"true\"></i> Edit</button> ";
                    str += "<button type=\"button\" class=\"btn btn-outline-danger btn-sm delete-customer\" data-customer-id=" +
                        row.Id +
                        "><i class=\"fa fa-times\" aria-hidden=\"true\"></i> Delete</button>";

                    return str;
                },
                edit: true
            }
        ]
    });

    $("#btn-search").click(function () {
        table.bootstrapTable("refresh");
    });

    form.find(".save-button").on("click", (e) => {
        e.preventDefault();
        
        if (!form.valid()) {
            return;
        }
        
        $.ajax({
            url: "/Customer/CreateCustomer",
            type: "POST",
            data: form.serialize({ checkboxesAsBools: true })
        }).done(function (res) {
            if (res.status) {
                createModal.modal("hide");;
                table.bootstrapTable("refresh");
            } else {
                $.alert(res.msg);
            }
        }).fail(function (xhr) {
            $.alert(xhr.responseText);
        });
    });

    createModal.on("hidden.bs.modal", () => {
        createModal.find("form")[0].reset();
    });

    $(document).on("click", ".edit-customer", function (e) {
        var id = $(this).attr("data-customer-id");
        e.preventDefault();

        $.ajax({
            url: "/Customer/Edit?id=" + id,
            type: "GET",
            dataType: "html",
            success: function (content) {
                $("#CustomerEditModal div.modal-content").html(content);
            },
            error: function (xhr) {
                console.log(xhr.responseText);
            }
        });
    });

    $(document).on("click", ".delete-customer", function (e) {
        var id = $(this).attr("data-customer-id");
        e.preventDefault();

        $.confirm({
            title: "Delete Customer",
            content: "Are you sure to delete this record ?",
            type: "red",
            buttons: {
                cancel: {
                    text: "CANCEL",
                    btnClass: "btn-danger"
                },
                ok: {
                    text: "CONFIRM",
                    btnClass: "btn-primary",
                    keys: ["enter"],
                    action: function () {
                        $.ajax({
                            url: "/Customer/DeleteCustomer",
                            type: "POST",
                            data: { id: id }
                        }).done(function (res) {
                            if (res.status) {
                                table.bootstrapTable("refresh");
                            } else {
                                $.alert(res.msg);
                            }
                        }).fail(function (xhr) {
                            $.alert(xhr.responseText);
                        });
                    }
                }
            }
        });
    });
})(jQuery);