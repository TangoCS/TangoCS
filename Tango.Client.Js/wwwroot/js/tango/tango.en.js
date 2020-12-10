var localization = function () {
    var resources = {
        title: {
            systemError: 'System Error',
            javascriptError: 'Error javascript',
            noAccess: 'No Access',
            pageMissing: 'The page is missing',
            ajaxError: 'Error ajax'
        },
        text: {
            notLoggedSystem: 'You are not logged in to the system',
            insufficientPermissionsOperation: 'Insufficient permissions to perform the operation',
            linkGoMainPage: '<a href="/">Go to the main page</a>'
        }
    };

    var instance = {
        resources: resources
    };
    return instance;
}();