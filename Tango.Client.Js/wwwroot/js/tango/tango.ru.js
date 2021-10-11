var localization = function () {
    var resources = {
        title: {
            systemError: 'Системная ошибка',
            javascriptError: 'Ошибка javascript',
            noAccess: 'Нет доступа',
            pageMissing: 'Страница отсутствует',
            ajaxError: 'Ошибка ajax'
        },
        text: {
            notLoggedSystem: 'Вы не авторизованы в системе',
            insufficientPermissionsOperation: 'Недостаточно прав для выполнения операции',
			linkGoToMainPage: '<a href="/">Перейти на главную страницу</a>',
			serverUnavailable: 'Сервер недоступен или не отвечает.<br><br>Адрес: '
        }
    };

    var instance = {
        resources: resources
    };
    return instance;
}();