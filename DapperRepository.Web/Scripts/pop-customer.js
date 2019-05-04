
/*
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
*/

var layer;
layui.use(['layer', 'table', 'form'], function () {
    layer = layui.layer;
});

$(function () {
    $('#btn-create').click(function () {
        var res = validate();
        if (!res) {
            return false;
        }

        var postData = {
            Username: $.trim($('#Username').val()),
            Email: $.trim($('#Email').val()),
            RoleId: $('#RoleId').val(),
            Active: $('#Active').is(":checked")
        };

        $.ajax({
            url: "/Customer/CreateCustomer",
            type: "POST",
            data: postData
        }).done(function (data) {
            if (data.status) {
                parent.layui.table.reload('customer-id', {
                    where: {
                        Username: $.trim($('#search-name').val()),
                        Email: $.trim($('#search-email').val())
                    },
                    page: {
                        curr: 1
                    }
                });
                var index = parent.layer.getFrameIndex(window.name);
                parent.layer.close(index);
            } else {
                layer.msg(data.msg, {
                    time: 1500,
                    icon: 7,
                    offset: ['40%', '40%']
                });
            }
        }).fail(function (xhr) {
            layer.msg(xhr.responseText, {
                time: 1500,
                icon: 2,
                offset: ['40%', '40%']
            });
        });
    });

    $('#btn-edit').click(function () {
        var res = validate();
        if (!res) {
            return false;
        }

        var postData = {
            Id: $('#Id').val(),
            Username: $.trim($('#Username').val()),
            Email: $.trim($('#Email').val()),
            RoleId: $('#RoleId').val(),
            Active: $('#Active').is(":checked")
        };

        $.ajax({
            url: "/Customer/EditCustomer",
            type: "POST",
            data: postData
        }).done(function (data) {
            if (data.status) {
                if (data.NeedReloadHomePage) {
                    parent.parent.location.reload();
                } else {
                    parent.layui.table.reload('customer-id', {
                        where: {
                            Username: $.trim($('#search-name').val()),
                            Email: $.trim($('#search-email').val())
                        },
                        page: {
                            curr: 1
                        }
                    });
                }
                var index = parent.layer.getFrameIndex(window.name);
                parent.layer.close(index);

            } else {
                layer.msg(data.msg, {
                    time: 1500,
                    icon: 7,
                    offset: ['40%', '40%']
                });
            }
        }).fail(function (xhr) {
            layer.msg(xhr.responseText, {
                time: 1500,
                icon: 2,
                offset: ['40%', '40%']
            });
        });
    });
});

function validate() {

    if ($.trim($('#Username').val()) === "") {
        layer.msg('Username is required', {
            time: 1500,
            icon: 5,
            offset: ['40%', '40%']
        });
        return false;
    }

    if ($.trim($('#Email').val()) === "") {
        layer.msg('Email is required', {
            time: 1500,
            icon: 5,
            offset: ['40%', '40%']
        });
        return false;
    }

    var reg = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    if (!reg.test($('#Email').val().trim())) {
        layer.msg('The Email format is incorrect.', {
            time: 1500,
            icon: 5,
            offset: ['40%', '40%']
        });
        return false;
    }

    return true;
}