var backgroundworker = function () {
    const connections = {};
    const notifications = {};

    var instance = {
        init: function (args) {
            const hub = 'backgroundworker';
            const task = args.taskUrl.toLowerCase();
            const elid = args.logContainer + '_body';
            const linkid = args.notificationContainer;

            notifications[task] = { link: linkid, log: elid };

            if (!connections[hub]) {
                const connection = new signalR.HubConnectionBuilder()
                    .withUrl('/' + hub)
                    .configureLogging(signalR.LogLevel.Information)
                    .build();
                connections[hub] = connection;

                connection.on('init', function (taskid, val) {
                    incCounter();
                    notifications[taskid].itemsCnt = parseInt(val);
                    appendLine(notifications[taskid].log, "init items " + val);
                });

                connection.on('complete', function (taskid) {
                    appendLine(notifications[taskid].log, "complete");
                    decCounter();
                });

                connection.on('progress', function (taskid, val) {
                    const link = document.getElementById(notifications[taskid].link);
                    const p = Math.round(parseInt(val) / notifications[taskid].itemsCnt * 100);
                    link.firstElementChild.firstElementChild.textContent = p + '%';
                    link.lastElementChild.firstElementChild.style.width = p + '%';
                });

                connection.on('message', function (taskid, message) {
                    appendLine(notifications[taskid].log, message);
                });

                connection.onclose(function (e) {
                    console.log('connection closed');
                });

                connection.start().then(function () {
                    console.log('connection started');
                    connection.invoke("GetConnectionId").then(function (connid) {
                        $.get(task + '?connid=' + connid);
                    });
                })
                    .catch(function (error) {
                        console.error(error.message);
                    });
            }
            else {
                connections[hub].invoke("GetConnectionId").then(function (connid) {
                    $.get(task + '?connid=' + connid);
                });
            }
        }
    };

    function incCounter() {
        const el = document.getElementById('backgroundworker_counter');
        el.innerText = parseInt(el.innerText) + 1;
        el.classList.remove('hide');
    }

    function decCounter() {
        const el = document.getElementById('backgroundworker_counter');
        el.innerText = parseInt(el.innerText) - 1;
        if (el.innerText == 0)
            el.classList.add('hide');
    }

    function appendLine(id, text) {
        const el = document.getElementById(id);
        const div = document.createElement("div");
        div.innerText = text;
        el.appendChild(div);
    }

    return instance;
}();