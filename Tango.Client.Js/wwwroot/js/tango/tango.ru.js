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
            linkGoMainPage: '<a href="/">Перейти на главную страницу</a>'
        }
    };

    var instance = {
        resources: resources
    };
    return instance;
}();