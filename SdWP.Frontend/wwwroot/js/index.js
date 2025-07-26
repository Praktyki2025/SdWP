window.initializeDataTable = function () {
    const table = window.projectsDataTable.initialize('#projectsViewTable');
    window.projectsEvents.register('#projectsViewTable', table);
};
