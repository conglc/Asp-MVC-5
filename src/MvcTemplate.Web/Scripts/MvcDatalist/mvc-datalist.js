﻿/*!
 * Datalist 4.1.1
 * https://github.com/NonFactors/MVC5.Datalist
 *
 * Copyright © NonFactors
 *
 * Licensed under the terms of the MIT License
 * http://www.opensource.org/licenses/mit-license.php
 */
(function ($) {
    $.widget('mvc.datalist', {
        _create: function () {
            if (!this.element.hasClass('datalist-input')) {
                return;
            }

            this._initOptions();
            this._initFilters();
            this._initAutocomplete();
            this._initDatalistOpenSpan();

            this._loadSelected();
            this._cleanUp();
        },
        _initOptions: function () {
            var e = this.element;
            var o = this.options;

            o.hiddenElement = $('#' + e.attr('data-datalist-for'))[0];
            o.sortColumn = e.attr('data-datalist-sort-column');
            o.sortOrder = e.attr('data-datalist-sort-order');
            o.page = parseInt(e.attr('data-datalist-page'));
            var filters = e.attr('data-datalist-filters');
            o.filters = filters ? filters.split(',') : [];
            o.search = e.attr('data-datalist-search');
            o.title = e.attr('data-datalist-title');
            o.rows = e.attr('data-datalist-rows');
            o.url = e.attr('data-datalist-url');
            e.addClass('mvc-datalist');
        },
        _initFilters: function () {
            for (var i = 0; i < this.options.filters.length; i++) {
                this._initFilter($('#' + this.options.filters[i]));
            }
        },
        _initFilter: function (filter) {
            var that = this;
            that._on(filter, {
                change: function () {
                    var event = $.Event(that._select);
                    if (that.options.filterChange) {
                        that.options.filterChange(event, that.element[0], that.options.hiddenElement, filter[0]);
                    }

                    if (!event.isDefaultPrevented()) {
                        that._select(null, false);
                    }
                }
            });
        },
        _initAutocomplete: function () {
            var that = this;
            this.element.autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: that._formAutocompleteUrl(request.term),
                        success: function (data) {
                            response($.map(data.Rows, function (item) {
                                return {
                                    label: item.DatalistAcKey,
                                    value: item.DatalistAcKey,
                                    item: item
                                };
                            }));
                        }
                    });
                },
                select: function (e, selection) {
                    that._select(selection.item.item, false);
                    e.preventDefault();
                },
                minLength: 1,
                delay: 500
            });

            this.element.on('keyup.datalist', function (e) {
                if (e.which != 9 && this.value.length == 0 && $(that.options.hiddenElement).val()) {
                    that._select(null, false);
                }
            });
            this.element.prevAll('.ui-helper-hidden-accessible').remove();
        },
        _initDatalistOpenSpan: function () {
            var datalistAddon = this.element.nextAll('.datalist-open-span:first');
            if (datalistAddon.length != 0) {
                var that = this;

                this._on(datalistAddon, {
                    click: function () {
                        var timeout;
                        datalist
                            .find('.datalist-search-input')
                            .off('keyup.datalist')
                            .on('keyup.datalist', function () {
                                var input = this;
                                clearTimeout(timeout);
                                timeout = setTimeout(function () {
                                    that.options.search = input.value;
                                    that.options.page = 0;
                                    that._update(datalist);
                                }, 500);
                            })
                            .val(that.options.search);
                        datalist
                            .find('.datalist-items-per-page')
                            .spinner({
                                change: function () {
                                    this.value = that._limitTo(this.value, 1, 99);
                                    that.options.rows = this.value;
                                    that.options.page = 0;
                                    that._update(datalist);
                                }
                            })
                            .val(that._limitTo(that.options.rows, 1, 99));

                        datalist.find('.datalist-search-input').attr('placeholder', $.fn.datalist.lang.Search);
                        datalist.find('.datalist-error-span').html($.fn.datalist.lang.Error);
                        datalist.dialog('option', 'title', that.options.title);
                        datalist.find('.datalist-table-head').empty();
                        datalist.find('.datalist-table-body').empty();
                        that._update(datalist);

                        setTimeout(function () {
                            var dialog = datalist.dialog('open').parent();

                            if (parseInt(dialog.css('left')) < 0) {
                                dialog.css('left', 0);
                            }
                            if (parseInt(dialog.css('top')) > 100) {
                                dialog.css('top', '100px');
                            }
                            else if (parseInt(dialog.css('top')) < 0) {
                                dialog.css('top', 0);
                            }
                        }, 100);
                    }
                });
            }
        },

        _formAutocompleteUrl: function (search) {
            return this.options.url +
                '?Search=' + search +
                '&SortOrder=Asc' +
                '&Rows=20' +
                '&Page=0' +
                this._formFiltersQuery();
        },
        _formDatalistUrl: function (search) {
            return this.options.url +
                '?Search=' + search +
                '&SortColumn=' + this.options.sortColumn +
                '&SortOrder=' + this.options.sortOrder +
                '&Rows=' + this.options.rows +
                '&Page=' + this.options.page +
                this._formFiltersQuery();
        },
        _formFiltersQuery: function () {
            var additionaFilter = '';
            for (var i = 0; i < this.options.filters.length; i++) {
                var filter = $('#' + this.options.filters[i]);
                if (filter.length == 1) {
                    additionaFilter += '&' + this.options.filters[i] + '=' + filter.val();
                }
            }

            return additionaFilter;
        },

        _defaultSelect: function (data, firstLoad) {
            if (data) {
                $(this.options.hiddenElement).val(data.DatalistIdKey);
                $(this.element).val(data.DatalistAcKey);
            } else {
                $(this.options.hiddenElement).val(null);
                $(this.element).val(null);
            }

            if (!firstLoad) {
                $(this.options.hiddenElement).change();
                $(this.element).change();
            }
        },
        _loadSelected: function () {
            var that = this;
            var id = $(that.options.hiddenElement).val();
            if (id) {
                $.ajax({
                    url: that.options.url + '?Id=' + id + '&Rows=1' + this._formFiltersQuery(),
                    cache: false,
                    success: function (data) {
                        if (data.Rows.length > 0) {
                            that._select(data.Rows[0], true);
                        }
                    }
                });
            }
        },
        _select: function (data, firstLoad) {
            var event = $.Event(this._defaultSelect);
            if (this.options.select) {
                this.options.select(event, this.element[0], this.options.hiddenElement, data, firstLoad);
            }

            if (!event.isDefaultPrevented()) {
                this._defaultSelect(data, firstLoad);
            }
        },

        _limitTo: function (value, min, max) {
            value = parseInt(value);
            if (isNaN(value)) {
                return 20;
            }

            if (value < min) {
                return min;
            }

            if (value > max) {
                return max;
            }

            return value;
        },
        _cleanUp: function () {
            this.element.removeAttr('data-datalist-sort-column');
            this.element.removeAttr('data-datalist-sort-order');
            this.element.removeAttr('data-datalist-filters');
            this.element.removeAttr('data-datalist-search');
            this.element.removeAttr('data-datalist-title');
            this.element.removeAttr('data-datalist-rows');
            this.element.removeAttr('data-datalist-page');
            this.element.removeAttr('data-datalist-url');
        },

        _update: function (datalist) {
            var that = this;
            var search = datalist.find('.datalist-search-input').val();
            datalist.find('.datalist-error-container').fadeOut(300);

            var timeout = setTimeout(function () {
                datalist.find('.datalist-processing').fadeIn(300);
                datalist.find('.datalist-pager').fadeOut(300);
                datalist.find('.datalist-data').fadeOut(300);
            }, 500);

            $.ajax({
                url: that._formDatalistUrl(search),
                cache: false,
                success: function (data) {
                    that._updateHeader(datalist, data.Columns);
                    that._updateData(datalist, data);
                    that._updateNavbar(datalist, data.FilteredRows);

                    clearTimeout(timeout);
                    datalist.find('.datalist-processing').fadeOut(300);
                    datalist.find('.datalist-error-container').hide();
                    datalist.find('.datalist-pager').fadeIn(300);
                    datalist.find('.datalist-data').fadeIn(300);
                },
                error: function () {
                    clearTimeout(timeout);
                    datalist.find('.datalist-error-container').fadeIn(300);
                    datalist.find('.datalist-processing').hide();
                    datalist.find('.datalist-pager').hide();
                    datalist.find('.datalist-data').hide();
                }
            });
        },
        _updateHeader: function (datalist, columns) {
            var that = this;
            var header = '';

            for (var i = 0; i < columns.length; i++) {
                var column = columns[i];
                if (column.Hidden) {
                    continue;
                }

                header += '<th class="' + (column.CssClass || '') + '" data-column="' + column.Key + '"><span class="datalist-header-title">' + (column.Header || '') + '</span>';
                if (that.options.sortColumn == column.Key || (that.options.sortColumn == '' && i == 0)) {
                    header += '<span class="datalist-sort-arrow ' + (that.options.sortOrder == 'Asc' ? 'asc' : 'desc') + '"></span></th>';
                    that.options.sortColumn = column.Key;
                } else {
                    header += '<span class="datalist-sort-arrow"></span></th>';
                }
            }

            datalist.find('.datalist-table-head').html('<tr>' + header + '<th class="datalist-select-header"></th></tr>');
            datalist.find('.datalist-table-head th').click(function () {
                var header = $(this);
                if (!header.attr('data-column')) {
                    return false;
                }

                if (that.options.sortColumn == header.attr('data-column')) {
                    that.options.sortOrder = that.options.sortOrder == 'Asc' ? 'Desc' : 'Asc';
                } else {
                    that.options.sortOrder = 'Asc';
                }

                that.options.sortColumn = header.attr('data-column');
                that._update(datalist);
            });
        },
        _updateData: function (datalist, data) {
            if (data.Rows.length == 0) {
                var columns = (data.Columns) ? data.Columns.length + 1 : 1;
                datalist.find('.datalist-table-body').html('<tr><td colspan="' + columns + '" style="text-align: center">' + $.fn.datalist.lang.NoData + '</td></tr>');

                return;
            }

            var tableData = '';
            for (var i = 0; i < data.Rows.length; i++) {
                var tableRow = '<tr>';
                var row = data.Rows[i];

                for (var j = 0; j < data.Columns.length; j++) {
                    var column = data.Columns[j];
                    if (column.Hidden) {
                        continue;
                    }

                    tableRow += '<td class="' + (column.CssClass || '') + '">' + (row[column.Key] || '') + '</td>';
                }

                tableRow += '<td class="datalist-select-cell"><div class="datalist-select-container"><i></i></div></td></tr>';
                tableData += tableRow;
            }

            datalist.find('.datalist-table-body').html(tableData);
            var selectRows = datalist.find('.datalist-table-body tr');
            for (var k = 0; k < selectRows.length; k++) {
                this._bindSelect(datalist, selectRows[k], data.Rows[k]);
            }
        },
        _updateNavbar: function (datalist, filteredRows) {
            var pageLength = datalist.find('.datalist-items-per-page').val();
            var totalPages = parseInt(filteredRows / pageLength) + 1;
            if (filteredRows % pageLength == 0) {
                totalPages--;
            }

            if (totalPages == 0) {
                datalist.find('.datalist-pager > .pagination').empty();
            } else {
                this._paginate(totalPages);
            }
        },
        _paginate: function (totalPages) {
            var startingPage = Math.floor(this.options.page / 5) * 5;
            var currentPage = this.options.page;
            var page = startingPage;
            var pagination = '';
            var that = this;

            if (totalPages > 5 && currentPage > 0) {
                pagination = '<li><span data-page="0">&laquo;</span></li><li><span data-page="' + (currentPage - 1) + '">&lsaquo;</span></li>';
            }

            while (page < totalPages && page < startingPage + 5) {
                var liClass = '';
                if (page == this.options.page) {
                    liClass = ' class="active"';
                }

                pagination += '<li' + liClass + '><span data-page="' + page + '">' + (++page) + '</span></li>';
            }

            if (totalPages > 5 && currentPage < (totalPages - 1)) {
                pagination += '<li><span data-page="' + (currentPage + 1) + '">&rsaquo;</span></li><li><span data-page="' + (totalPages - 1) + '">&raquo;</span></li>';
            }

            datalist.find('.datalist-pager > .pagination').html(pagination).find('li:not(.active) > span').click(function (e) {
                that.options.page = parseInt($(this).data('page'));
                that._update(datalist);
            });
        },
        _bindSelect: function (datalist, selectRow, data) {
            var that = this;
            that._on(selectRow, {
                click: function () {
                    datalist.dialog('close');
                    that._select(data, false);
                }
            });
        },

        _destroy: function () {
            var e = this.element;
            var o = this.options;

            e.attr('data-datalist-filters', o.filters.join());
            e.attr('data-datalist-sort-column', o.sortColumn);
            e.attr('data-datalist-sort-order', o.sortOrder);
            e.attr('data-datalist-search', o.search);
            e.attr('data-datalist-title', o.title);
            e.attr('data-datalist-rows', o.rows);
            e.attr('data-datalist-page', o.page);
            e.attr('data-datalist-url', o.url);
            e.removeClass('mvc-datalist');
            e.autocomplete('destroy');

            return this._super();
        }
    });

    $.fn.datalist.lang = {
        Error: 'Error while retrieving records',
        NoData: 'No data found',
        Search: 'Search...'
    };

    var datalist = $('#Datalist');

    $(function () {
        datalist.find('.datalist-items-per-page').spinner({ min: 1, max: 99 });
        datalist.dialog({
            dialogClass: 'datalist-dialog',
            autoOpen: false,
            minHeight: 210,
            height: 'auto',
            minWidth: 455,
            width: 'auto',
            modal: true
        });

        $('.datalist-input').datalist();
    });
})(jQuery);
