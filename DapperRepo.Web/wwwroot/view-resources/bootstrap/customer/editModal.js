(function ($) {
    var table = $("#customer-table"),
        editModal = $("#CustomerEditModal"),
        form = editModal.find("form");

    function save() {
        if (!form.valid()) {
            return;
        }
        
        $.ajax({
            url: "/Customer/EditCustomer",
            type: "POST",
            data: form.serialize({ checkboxesAsBools: true })
        }).done(function (res) {
            if (res.status) {
                editModal.modal("hide");;
                table.bootstrapTable("refresh");
            } else {
                $.alert(res.msg);
            }
        }).fail(function (xhr) {
            $.alert(xhr.responseText);
        });
    }

    form.closest("div.modal-content").find(".save-button").click(function (e) {
        e.preventDefault();
        save();
    });

    form.find("input").on("keypress", function (e) {
        if (e.which === 13) {
            e.preventDefault();
            save();
        }
    });

    editModal.on("shown.bs.modal", function () {
        form.find("input[type=text]:first").focus();
    });

}(jQuery));