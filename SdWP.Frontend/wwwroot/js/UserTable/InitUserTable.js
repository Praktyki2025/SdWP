window.initializeUserTable = function () {
    const table = window.userDataTable.init('#userTable');
    window.usersEvents.register('#userTable', table);

    table.on('draw.dt', function () {
        $('#userTable td.sorting_1').removeClass('sorting_1');
    });
};