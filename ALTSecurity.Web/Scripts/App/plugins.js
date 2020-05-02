var modalWindowButtons = {
    None: 0,
    Ok: 1,
    Cancel: 2,
    OkCancel: 3
};

var modalWindowSize = {
    small: 0,
    medium: 1,
    large: 2,
    extraLarge: 3
};

var modalResult = {
    ok: 0,
    cancel: 1
};

(function ($) {
    //Динамическая подгрузка модального окна
    $.ajaxModalWindow = function (url, opts, cache) {
        if (url) {
            if (!opts.type) {
                opts.type = 'POST';
            }

            var modal = null;

            if (cache && $('.modal[data-url="' + url + '"]').length > 0) {
                modal = $('.modal[data-url="' + url + '"]');
                modal.modal();
            }
            else {
                $.mask('Зачекайте...');
                $.ajax({
                    url: url,
                    type: opts.type,
                    data: opts.data,
                    success: function (res) {
                        if (res) {
                            modal = $(res);

                            modal.attr("data-url", url);
                            $('body').append(modal);

                            $('.modal-footer [type="button"]', modal).on('click', function () {
                                if (opts.success) {
                                    var $this = $(this);
                                    var result = parseInt($this.data('result'));
                                    opts.success($this.parents(':eq(3)'), result);
                                }
                                else {
                                    modal.modal('hide');
                                }
                            });

                            if (opts.ready) {
                                opts.ready(modal);
                            }

                            if (!cache) {
                                modal.on('hidden.bs.modal', function () {
                                    modal.remove();
                                });
                            }

                            modal.modal();
                        }
                    },
                    complete: function () {
                        $.unmask();
                    }
                });
            }
        }
    };

    //Построение модального окна на клиенте
    $.modalWindow = function (id, title, body, buttons, size, scrollable, success) {
        if (!id || !body) {
            return;
        }

        if (!size) {
            size = modalWindowSize.medium;
        }

        var markup = '';
        var dialogClass = '';

        markup += '<div id = \"' + id + '\" class=\"modal fade\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"' + id + '\" aria-hidden=\"true\">';

        switch (size) {
            case modalWindowSize.small:
                dialogClass = 'modal-dialog modal-sm';
                break;
            case modalWindowSize.medium:
                dialogClass = 'modal-dialog';
                break;
            case modalWindowSize.large:
                dialogClass = 'modal-dialog modal-lg';
                break;
            case modalWindowSize.extraLarge:
                dialogClass = 'modal-dialog modal-xl';
                break;
            default:
                break;
        }

        if (scrollable) {
            dialogClass += ' modal-dialog-scrollable';
        }

        markup += '<div class=\"' + dialogClass + '\" role=\"document\">';
        markup += '<div class=\"modal-content\">';

        markup += '<div class=\"modal-header\">';
        markup += '<h5 class=\"modal-title\" id=\"' + id + 'Label' + '\">' + title + '</h5>';
        markup += '<button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Close\">';
        markup += '<span aria-hidden=\"true\">&times;</span>';
        markup += '</button>';
        markup += '</div>';

        markup += '<div class=\"modal-body\">';
        markup += body;
        markup += '</div>';

        var footerContent = '';

        switch (buttons) {
            case modalWindowButtons.Ok:
                footerContent = '<button type=\"button\" data-result=\"' + modalResult.ok + '\" class=\"btn btn-primary\">Ок</button>';
                break;
            case modalWindowButtons.Cancel:
                footerContent = '<button type=\"button\" data-result=\"' + modalResult.cancel + '\" class=\"btn btn-primary\">Закрити</button>';
                break;
            case modalWindowButtons.OkCancel:
                footerContent += '<button type=\"button\" data-result=\"' + modalResult.ok + '\" class=\"btn btn-primary\">Ок</button>';
                footerContent += '<button type=\"button\" data-result=\"' + modalResult.cancel + '\" class=\"btn btn-secondary\">Закрити</button>';
                break;
            default:
                break;
        }

        markup += '<div class=\"modal-footer\">';
        markup += footerContent;
        markup += '</div>';

        markup += '</div>';

        var modal = $(markup);
        $('.modal-footer [type="button"]', modal).on('click', function () {
            if (success) {
                var $this = $(this);
                var result = parseInt($this.data('result'));
                success($this.parents(':eq(3)'), result);
            }
            else {
                modal.modal('hide');
            }
        });

        return modal;
    };

    $.fn.modalReady = function (callback) {
        var modal = $(this);
        callback(modal);
    };

    // Показать простое сообщение
    $.modalAlert = function (message, success) {
        var modal = $.modalWindow('alertWindow', 'Повідомлення', message, modalWindowButtons.Ok, modalWindowSize.small, false, success);
        modal.on('hidden.bs.modal', function () {
            modal.remove();
        });

        modal.modal();
    };

    //Окно для подтверждения действия
    $.modalConfirm = function (message, success) {
        message = message ? message : 'Ви дійсно бажаєте продовжити?';

        var modal = $.modalWindow('confirmWindow', 'Підтвердження', message, modalWindowButtons.OkCancel, modalWindowSize.medium, false, success);
        modal.on('hidden.bs.modal', function () {
            modal.remove();
        });

        modal.modal();
    };

    ////Окно с простым полем ввода
    //$.modalPrompt = function (data, message, title, success) {

    //};


}(jQuery));