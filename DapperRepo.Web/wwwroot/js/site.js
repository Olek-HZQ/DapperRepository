// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function ($) {

    $.fn.serialize = function (options) {
        return $.param(this.serializeArray(options));
    };

    $.fn.serializeArray = function (options) {
        var o = $.extend({
            checkboxesAsBools: false
        }, options || {});

        var selectTextarea = /select|textarea/i;
        var input = /text|email|hidden|password|search/i;

        return this.map(function () {
                return this.elements ? $.makeArray(this.elements) : this;
            })
            .filter(function () {
                return this.name && !this.disabled &&
                (this.checked
                    || (o.checkboxesAsBools && this.type === "checkbox")
                    || selectTextarea.test(this.nodeName)
                    || input.test(this.type));
            })
            .map(function (i, elem) {
                var val = $(this).val();
                return val == null ?
                    null :
                    $.isArray(val) ?
                    $.map(val, function (value) {
                        return { name: elem.name, value: value };
                    }) :
                    {
                        name: elem.name,
                        value: (o.checkboxesAsBools && this.type === "checkbox") ? 
                            (this.checked ? "true" : "false") :
                            val
                    };
            }).get();
    };

})(jQuery);

function validateEmail(email) {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
}