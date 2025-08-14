window.initializeDataTable = function () {
    const table = window.projectsDataTable.initialize('#projectsViewTable');
    window.projectsEvents.register('#projectsViewTable', table);

    table.on('draw.dt', function () {
        $('#projectsViewTable td.sorting_1').removeClass('sorting_1');
    });
};