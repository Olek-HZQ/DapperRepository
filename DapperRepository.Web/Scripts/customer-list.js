var table, layer;
layui.use(['table', 'form', 'layer'], function () {
    table = layui.table;
    layer = layui.layer;
    var form = layui.form;

    form.on('checkbox(select-permission-filter)', function (data) {
        var obj = data.elem;
        $('.allow_' + obj.id).each(function () {
            this.checked = data.elem.checked;
        });
        form.render('checkbox');
    });

    table.render({
        elem: '#customer-table',
        id: 'customer-id',
        toolbar: '#customer-toolbar',
        url: '/Customer/CustomerList',
        request: {
            pageName: 'PageIndex',
            limitName: 'PageSize'
        },
        page: true,
        cols: [[
            {
                field: 'Id',
                title: 'ID',
                sort: true
            },
            {
                field: 'Username',
                title: 'UserName'
            },
            {
                field: 'Email',
                title: 'Email'
            },
            {
                field: 'RoleName',
                title: 'Role'
            },
            {
                field: 'Active',
                title: 'Active',
                templet: function(d) {
                    if (d.Active) {
                        return '<i class="layui-icon">&#xe605;</i>  ';
                    } else {
                        return '<i class="layui-icon">&#x1006;</i>  ';
                    }
                }
            },
            {
                field: 'CreationTime',
                title: 'CreationTime',
                sort: true
            },
            {
                fixed: 'right',
                width: 300,
                align: 'center',
                toolbar: '#customer-right-bar'
            }
        ]]
    });

    table.on('tool(customer)', function (obj) {
        var data = obj.data, //获得当前行数据
            layEvent = obj.event;

        switch (layEvent) {
            case 'edit':
                layer.open({
                    type: 2,
                    title: 'Edit Customer',
                    maxmin: true,
                    shade: 0.5,
                    area: ["50%", "50%"],
                    content: '/Customer/PopCustomer?viewName=_CustomerEdit&id=' + data.Id,
                    success: function () {
                        //setTimeout(function () {
                        //    layui.layer.tips('Click to return customer list',
                        //        '.layui-layer-setwin .layui-layer-close',
                        //        {
                        //            tips: 3
                        //        });
                        //}, 500);
                    }
                });
                break;
            case 'delete':
                layer.confirm('Are you sure to delete this data?', {
                    btn: ['OK', 'Cancel']
                }, function (index) {

                    var postData = {
                        id: data.Id
                    };

                    $.ajax({
                        url: "/Customer/DeleteCustomer",
                        type: "POST",
                        data: postData
                    }).done(function (result) {
                        if (result.status) {
                            table.reload('customer-id', {
                                where: {
                                    Username: $.trim($('#search-name').val()),
                                    Email: $.trim($('#search-email').val())
                                },
                                page: {
                                    curr: 1
                                }
                            });
                            layer.close(index);
                        } else {
                            layer.msg(result.msg, {
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
                break;
        }
    });

    $('#btn-search').click(function () {
        table.reload('customer-id', {
            where: {
                Username: $.trim($('#search-name').val()),
                Email: $.trim($('#search-email').val())
            },
            page: {
                curr: 1
            }
        });
    });
});

function add() {
    layer.open({
        type: 2,
        title: 'Add Customer',
        maxmin: true,
        shade: 0.5,
        area: ["50%", "50%"],
        content: '/Customer/PopCustomer?viewName=_CustomerCreate',
        success: function () {
            //setTimeout(function () {
            //    layui.layer.tips('Click to return customer list',
            //        '.layui-layer-setwin .layui-layer-close',
            //        {
            //            tips: 3
            //        });
            //}, 500);
        }
    });
}