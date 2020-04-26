var app = {
    sectionInit: null,
    resources: null,
    init: function () {
        var scope = app;
        $.ajaxSetup({
            type: 'POST',
            beforeSend: function (jqXhr, settings) {
                settings.url = scope.resolveURL(settings.url);
            },
            statusCode: {
                200: function (data, status, jqXhr) {
                    try {
                        if (jqXhr && jqXhr.getResponseHeader) {
                            var xRespondedJsonStr = jqXhr.getResponseHeader('x-responded-json');
                            if (xRespondedJsonStr) {
                                var xRespondedJsonObj = JSON.parse(xRespondedJsonStr);
                                if (xRespondedJsonObj.status === 401) {
                                    window.location = scope.resolveURL("Account/Login");
                                }
                            }
                        }
                    } catch (e) {
                        console.log(e);
                    }
                },
                401: function () {
                    window.location = scope.resolveURL("Account/Login");
                },
                403: function () {
                    alert("You have no enough permissions to request this resource.");
                },
                500: function () {
                    //window.location.reload();
                }
            }
        });

        function GetAntiForgeryToken() {
            var tokenField = $("input[type='hidden'][name$='RequestVerificationToken']:first");
            if (tokenField.length === 0) {
                return null;
            } else {
                return {
                    name: tokenField[0].name,
                    value: tokenField[0].value
                };
            }
        }

        $.ajaxPrefilter(
            function (options, localOptions, jqXHR) {
                if (options.type !== "GET" && options.contentType !== 'application/json' && options.processData !== false && options.contentType !== false) {
                    var token = GetAntiForgeryToken();
                    if (token !== null) {
                        if (!options.data) {
                            options.data = "";
                        }
                        if (options.data.indexOf(token.name) === -1) {
                            options.data = options.data + "&" + token.name + '=' + token.value;
                        }
                    }
                }
            }
        );

        //Инициализация ресурсов локализации
        scope.resourcesInit();

        //Инициализация скриптов после главного объекта
        scope.sectionInit();
    },
    resourcesInit: function () {

    },
    resolveURL: function (url, crossDomain) {
        if (url.indexOf(document.location.host) !== -1) {
            url = url.substr(url.indexOf(document.location.host) + document.location.host.length);
        }
        while (url.substr(0, 1) === '/') url = url.substr(1);
        if (!crossDomain) {
            url = '/' + url;
            if (!this.baseUrl)
                this.baseUrl = '/';
            if (url.substr(0, this.baseUrl.length) !== this.baseUrl) {
                url = this.baseUrl + url.substr(1);
            }
        }
        return url.toLowerCase();
    }
};