window.initializeDataTable = () => {

    document.getElementById('addProjectBtn').onclick = function () {
        window.location.href = '/projects/add';
    };

    $('#projectsViewTable').DataTable({
        processing: true,
        serverSide: true,
        ajax: function (data, callback, settings) {
            const mockData = {
                draw: data.draw,
                recordsTotal: 3,
                recordsFiltered: 3,
                data: [
                    { Id: 1, Title: "Mock data 1", Description: "Mock data 1" },
                    { Id: 2, Title: "Mock data 2", Description: "Mock data 2" },
                    { Id: 3, Title: "Mock data 3", Description: "Mock data 3" }
                ]
            };
            callback(mockData);
        },
        columns: [
            { data: "Title" },
            { data: "Description" },
            {
                data: "Id",
                orderable: false,
                searchable: false,
                render: function (data, type, row, meta) {
                    return `
                        <div class="dropdown">
                            <button class="btn btn-sm btn-outline-secondary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                <i class="fas fa-cog"></i> Actions
                            </button>
                            <ul class="dropdown-menu">
                                <li>
                                    <a class="dropdown-item edit-project" href="#" data-id="${data}">
                                        <i class="fas fa-edit me-2"></i>Edit
                                    </a>
                                </li>
                                <li>
                                    <a class="dropdown-item text-danger delete-project" href="#" data-id="${data}">
                                        <i class="fas fa-trash-alt me-2"></i>Delete
                                    </a>
                                </li>
                            </ul>
                        </div>
                    `;
                }
            }
        ]
    });

    $('#projectsViewTable').on('click', '.edit-project', function (e) {
        e.preventDefault();
        const id = $(this).data('id');
        window.location.href = `/projects/edit?id=${id}`;
    });

    $('#projectsViewTable').on('click', '.delete-project', function (e) {
        e.preventDefault();
        const id = $(this).data('id');
        if (confirm('Are you sure you want to delete this project?')) {
            projectsData = projectsData.filter(p => p.Id !== id);
            table.clear().rows.add(projectsData).draw();
        }
    });

    // click on row to go to valuations
    $('#projectsViewTable tbody').on('click', 'tr', function (e) {
        if ($(e.target).closest('.dropdown, .dropdown-menu').length === 0) {
            window.location.href = '/projects/valuations';
        }
    });
};
