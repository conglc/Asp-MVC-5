// Header dropdown closure
(function () {
    $(document).on('mouseleave', '.header-navigation .dropdown', function () {
        $(this).removeClass('open');
    });
}());

// Alert closure
(function () {
    $('.alerts .alert').each(function () {
        var alert = $(this);

        if (alert.data('timeout')) {
            setTimeout(function () {
                alert.fadeTo(300, 0).slideUp(300, function () {
                    $(this).remove();
                });
            }, alert.data('timeout'));
        }
    });

    $(document).on('click', '.alert .close', function () {
        $(this.parentNode).fadeTo(300, 0).slideUp(300, function () {
            $(this).remove();
        });
    });
}());

// JQuery dialog overlay binding
(function () {
    window.addEventListener('mousedown', function (e) {
        if ($(e.target || e.srcElement).hasClass('ui-widget-overlay')) {
            $('.ui-dialog-content:visible').dialog('close');

            e.stopImmediatePropagation();
        }
    }, true);
}());

// Globalized validation binding
(function () {
    $.validator.methods.date = function (value, element) {
        return this.optional(element) || Globalize.parseDate(value);
    };

    $.validator.methods.number = function (value, element) {
        var pattern = new RegExp('^(?=.*\\d+.*)[-+]?\\d*[' + Globalize.culture().numberFormat['.'] + ']?\\d*$');

        return this.optional(element) || pattern.test(value);
    };

    $.validator.methods.min = function (value, element, param) {
        return this.optional(element) || Globalize.parseFloat(value) >= parseFloat(param);
    };

    $.validator.methods.max = function (value, element, param) {
        return this.optional(element) || Globalize.parseFloat(value) <= parseFloat(param);
    };

    $.validator.methods.range = function (value, element, param) {
        return this.optional(element) || (Globalize.parseFloat(value) >= parseFloat(param[0]) && Globalize.parseFloat(value) <= parseFloat(param[1]));
    };

    $.validator.addMethod('greater', function (value, element, param) {
        return this.optional(element) || Globalize.parseFloat(value) > parseFloat(param);
    });
    $.validator.unobtrusive.adapters.add("greater", ["min"], function (options) {
        options.rules["greater"] = options.params.min;
        if (options.message) {
            options.messages["greater"] = options.message;
        }
    });

    $.validator.addMethod('integer', function (value, element) {
        return this.optional(element) || /^[+-]?\d+$/.test(value);
    });
    $.validator.unobtrusive.adapters.addBool("integer");

    $.validator.addMethod('filesize', function (value, element, param) {
        if (this.optional(element) || !element.files)
            return true;

        var bytes = 0;
        for (var i = 0; i < element.files.length; i++) {
            bytes += element.files[i].size;
        }

        return bytes <= parseFloat(param);
    });
    $.validator.unobtrusive.adapters.add("filesize", ["max"], function (options) {
        options.rules["filesize"] = options.params.max;
        if (options.message) {
            options.messages["filesize"] = options.message;
        }
    });
    $(document).on('change', '[type="file"]', function () {
        $(this).focusout();
    });

    $(document).on('change', '.datalist-hidden-input', function () {
        var validator = $(this).parents('form').validate();

        if (validator) {
            var datalist = $(this).prevAll('[data-datalist-for="' + this.id + '"]');
            if (validator.element('#' + this.id)) {
                datalist.removeClass('input-validation-error');
            } else {
                datalist.addClass('input-validation-error');
            }
        }
    });
    $('form').on('invalid-form', function (form, validator) {
        var datalists = $(this).find('.datalist-input');
        for (var i = 0; i < datalists.length; i++) {
            var datalist = $(datalists[i]);
            var hiddenInputId = datalist.attr('data-datalist-for');

            if (validator.invalid[hiddenInputId]) {
                datalist.addClass('input-validation-error');
            } else {
                datalist.removeClass('input-validation-error');
            }
        }
    });
    $(document).on('ready', function () {
        var hiddenInputs = $('.datalist-hidden-input.input-validation-error');
        for (var i = 0; i < hiddenInputs.length; i++) {
            var hiddenInput = $(hiddenInputs[i]);
            hiddenInput.prevAll('[data-datalist-for="' + hiddenInputs[i].id + '"]').addClass('input-validation-error');
        }
    });

    var currentIgnore = $.validator.defaults.ignore;
    $.validator.setDefaults({
        ignore: function () {
            return $(this).is(currentIgnore) && !$(this).hasClass('datalist-hidden-input');
        }
    });

    var lang = $('html').attr('lang');

    Globalize.cultures.en = null;
    Globalize.addCultureInfo(lang, window.cultures.globalize[lang]);
    Globalize.culture(lang);
}());

// Datepicker binding
(function () {
    var lang = $('html').attr('lang');
    var options = {
        beforeShow: function (e) {
            return !$(e).attr('readonly');
        },
        onSelect: function () {
            $(this).focusout();
        }
    };

    if ($.fn.datepicker) {
        $.datepicker.setDefaults(window.cultures.datepicker[lang]);
        $(".datepicker").datepicker(options);
    }

    if ($.fn.timepicker) {
        $.timepicker.setDefaults(window.cultures.timepicker[lang]);
        $(".datetimepicker").datetimepicker(options);
    }
}());

// JsTree binding
(function () {
    var trees = $('.js-tree-view');
    for (var i = 0; i < trees.length; i++) {
        var tree = $(trees[i]).jstree({
            'core': {
                'themes': {
                    'icons': false
                }
            },
            'plugins': [
                'checkbox'
            ],
            'checkbox': {
                'keep_selected_style': false
            }
        });

        tree.on('ready.jstree', function (e, data) {
            var selected = $(this).prev('.js-tree-view-ids').children();
            for (var j = 0; j < selected.length; j++) {
                data.instance.select_node(selected[j].value, false, true);
            }

            data.instance.open_node($.makeArray(tree.find('> ul > li')), null, null);
            data.instance.element.show();
        });
    }

    $(document).on('submit', 'form', function () {
        var trees = $(this).find('.js-tree-view');
        for (var i = 0; i < trees.length; i++) {
            var tree = $(trees[i]).jstree();
            var ids = tree.element.prev('.js-tree-view-ids');

            ids.empty();
            var selected = tree.get_selected();
            for (var j = 0; j < selected.length; j++) {
                var node = tree.get_node(selected[j]);
                if (node.li_attr.id) {
                    ids.append('<input type="hidden" value="' + node.li_attr.id + '" name="' + tree.element.attr('for') + '" />');
                }
            }
        }
    });
}());

// Mvc.Grid binding
(function () {
    if ($.fn.mvcgrid) {
        $('.mvc-grid').mvcgrid();
        $.fn.mvcgrid.lang = window.cultures.grid[$('html').attr('lang')];

        if (MvcGridNumberFilter) {
            MvcGridNumberFilter.prototype.isValid = function (value) {
                var pattern = new RegExp('^(?=.*\\d+.*)[-+]?\\d*[' + Globalize.culture().numberFormat['.'] + ']?\\d*$');

                return value == '' || pattern.test(value);
            }
        }
    }
}());

// Datalist binding
(function () {
    if ($.fn.datalist) {
        $.fn.datalist.lang = window.cultures.datalist[$('html').attr('lang')];
    }
}());

// Read only binding
(function () {
    $(document).on('click', 'input:checkbox[readonly],input:radio[readonly]', function () {
        return false;
    });
}());

// Input focus binding
(function () {
    var invalidInput = $('.content-container .input-validation-error:visible:not([readonly],.datepicker,.datetimepicker):first');
    if (invalidInput.length > 0) {
        invalidInput.focus();
        invalidInput.val(invalidInput.val());
    } else {
        var input = $('.content-container input:text:visible:not([readonly],.datepicker,.datetimepicker):first');
        if (input.length > 0) {
            input.focus();
            input.val(input.val());
        }
    }
}());

// Bootstrap binding
(function () {
    $('body').tooltip({
        selector: '[data-toggle=tooltip]'
    });
}());
