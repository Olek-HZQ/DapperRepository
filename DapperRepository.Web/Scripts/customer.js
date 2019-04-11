$(function () {

    $('#customer-table').bootstrapTable({
        url: '/Customer/CustomerList',
        method: 'GET',
        cache: false,
        pagination: true,
        queryParams: function (e) {
            var param = {
                pageIndex: e.offset / e.limit + 1,
                pageSize: e.limit,
                username: $.trim($('#search-username').val()),
                email: $.trim($('#search-email').val())
            };
            return param;
        },
        sidePagination: "server",
        pageNumber: 1,
        pageSize: 10,
        uniqueId: "Id",
        columns: [
            {
                field: 'Id',
                title: 'ID'
            }, {
                field: 'Username',
                title: 'Username'
            }, {
                field: 'Email',
                title: 'Email'
            }, {
                field: "RoleName",
                title: "Role"
            }, {
                field: 'Active',
                title: 'Active',
                align: "center",
                formatter: function (value, row) {
                    if (value) {
                        return '<i class="fa fa-check"></i>';
                    } else {
                        return '<i class="fa fa-close"></i>';
                    }
                }
            }, {
                field: "CreationTime",
                title: "CreationTime"
            },
            {
                field: "Action",
                title: "Action",
                align: "center",
                formatter: action,
                edit: true
            }
        ]
    });

    $('#operationModal').on('hidden.bs.modal', function () {
        $('#Username').val('');
        $('#Email').val('');
        $('#Active').prop("checked", null);
    });

});

$('#btn-search').click(function () {
    $('#span-search-username').html('');
    $('#span-search-email').html('');

    var regSearchUsername = /[\u4e00-\u9fa5_a-zA-Z0-9_]{2,30}/;
    var searchUsername = $.trim($('#search-username').val());
    if (searchUsername !== '' && !regSearchUsername.test(searchUsername)) {
        $('#span-search-username').html('The search username range of length from 2 characters to 30.');
        $('#search-username').focus();
        return false;
    }

    var regSearchEmail = /[\u4e00-\u9fa5_a-zA-Z0-9_]{2,64}/;
    var searchEmail = $.trim($('#search-email').val().trim());
    if (searchEmail !== '' && !regSearchEmail.test(searchEmail)) {
        $('#span-search-email').html('The search email range of length from 2 characters to 64.');
        $('#search-email').focus();
        return false;
    }

    $("#customer-table").bootstrapTable('selectPage', 1);
});

function action(value, row) {
    var str = '';
    str += '<a class="edit" href="javascript:void(0);" onclick="editCustomer(' + row.Id + ')" title="Edit"><i class="fa fa-edit"></i></a>  ';
    str += '  <a style="margin-left:5px;" class="remove" btn-sm href="javascript:void(0);" onclick="deleteCustomer(' + row.Id + ')" title="Delete"><i class="fa fa-trash"></i></a>';

    return str;
}

function add() {
    $('#btnUpdate').hide();
    $('#btnAdd').show();
    $('#operationModalLabel').html("Add Customer");
}

function editCustomer(id) {
    $('#operationModal .modal-body').load('/Customer/EditModal/' + id, function () {
        $('#btnAdd').hide();
        $('#btnUpdate').show();
        $('#operationModalLabel').html("Update Customer");
        $('#operationModal').modal("show");
    });
}

$('#btnAdd').click(function () {
    var res = validate();
    if (!res) {
        return false;
    }

    var postData = {
        Username: $('#Username').val().trim(),
        Email: $('#Email').val().trim(),
        RoleId: $('#RoleId').val(),
        Active: $('#Active').is(":checked")
    };

    $.ajax({
        url: "/Customer/AddCustomer",
        type: "POST",
        data: postData
    }).done(function (data) {
        if (data.status) {
            $('#operationModal').modal('hide');
            $("#customer-table").bootstrapTable('selectPage', 1);
        } else {
            console.log(data.msg);
        }
    }).fail(function (xhr) {
        console.log(xhr.message);
    });
});

$('#btnUpdate').click(function () {
    var res = validate();
    if (!res) {
        return false;
    }

    var postData = {
        Id: $('#Id').val(),
        Username: $('#Username').val().trim(),
        Email: $('#Email').val().trim(),
        RoleId: $('#RoleId').val(),
        Active: $('#Active').is(":checked")
    };

    $.ajax({
        url: "/Customer/UpdateCustomer",
        type: "POST",
        data: postData
    }).done(function (data) {
        if (data.status) {
            $('#operationModal').modal('hide');
            $("#customer-table").bootstrapTable('selectPage', 1);
        } else {
            console.log(data.msg);
        }
    }).fail(function (xhr) {
        console.log(xhr.message);
    });
});

function validate() {
    var isValid = true;

    var reg = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;

    if ($('#Username').val().trim() === "") {
        $('#Username').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Username').css('border-color', 'lightgrey');
    }
    if ($('#Email').val().trim() === "") {
        $('#Email').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Email').css('border-color', 'lightgrey');
    }
    if (!reg.test($('#Email').val().trim())) {
        alert('The Email format is incorrect.');
        $('#Email').css('border-color', 'Red');
        isValid = false;
    }
    if (parseInt($('#RoleId').val()) <= 0) {
        $('#RoleId').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#RoleId').css('border-color', 'lightgrey');
    }
    return isValid;
}

function deleteCustomer(id) {
    if (confirm("Are you sure to delete the data?")) {
        $.ajax({
            url: "/Customer/Delete",
            type: "POST",
            data: { id: id }
        }).done(function (data) {
            if (data.status) {
                $("#customer-table").bootstrapTable('selectPage', 1);
            } else {
                console.log(data.msg);
            }
        }).fail(function (xhr) {
            console.log(xhr.message);
        });
    }
}

$('#insert-sample-customer').click(function () {
    var num = $('#insert-num').val();

    $.ajax({
        url: "/Customer/InsertSampleData",
        type: "POST",
        data: { num: num }
    }).done(function (data) {
        console.log("ExecuteResult：" + data.ExecuteResult);
        console.log("ExecuteTime：" + data.ExecuteTime);
        $("#customer-table").bootstrapTable('selectPage', 1);
    }).fail(function (xhr) {
        console.log(xhr.message);
    });
});