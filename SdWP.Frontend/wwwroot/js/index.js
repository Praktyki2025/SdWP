window.initializeDataTable = function () {
    const table = window.projectsDataTable.initialize('#projectsViewTable');
    window.projectsEvents.register('#projectsViewTable', table);

    table.on('draw.dt', function () {
        $('#projectsViewTable td.sorting_1').removeClass('sorting_1');
    });
};

window.initializeValuationtable = function () {
    const table2 = window.valuationsDataTable.initialize('#Valuationtable');

    table2.on('draw.dt', function () {
        $('#Valuationtable td.sorting_1').removeClass('sorting_1');
    });
};