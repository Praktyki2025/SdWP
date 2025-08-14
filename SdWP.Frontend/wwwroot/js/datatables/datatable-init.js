window.projectsDataTable = {
    initialize: function (tableSelector) {
        if ($.fn.DataTable.isDataTable(tableSelector)) {
            $(tableSelector).DataTable().clear().destroy();
        }

        const table = $(tableSelector).DataTable({
            processing: true,
            serverSide: true,
            ajax: {
                url: '/api/projects/',
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                },
                dataSrc: 'data'
            },
            columns: [
                { data: 'title' },
                { data: 'description' },
                {
                    data: 'id',
                    orderable: false,
                    searchable: false,
                    render: function (data) {
                        return `
                            <div class="dropdown">
                            <button class="btn btn-sm btn-outline-secondary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                <i class="fas fa-cog"></i> Actions
                            </button>
                            <ul class="dropdown-menu">
                                <li><a class="dropdown-item edit-project" href="#" data-id="${data}">
                                <i class="fas fa-edit me-2"></i>Edit
                                </a></li>
                                <li><a class="dropdown-item text-danger delete-project" href="#" data-id="${data}">
                                <i class="fas fa-trash-alt me-2"></i>Delete
                                </a></li>
                            </ul>
                            </div>`;
                    }
                }
            ]
        });

        return table;
    }
};
