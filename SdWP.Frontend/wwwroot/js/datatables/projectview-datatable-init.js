window.initializeDataTable = (dotNetRef) => {
    let projectsData = [];

    let table = $('#projectsViewTable').DataTable({
        processing: true,
        serverSide: true,
        ajax: function (data, callback) {
            fetch('/api/project/all')
                .then(response => response.json())
                .then(json => {
                    projectsData = json;
                    callback({
                        draw: data.draw,
                        recordsTotal: json.length,
                        recordsFiltered: json.length,
                        data: json
                    });
                })
                .catch(() => {
                    callback({
                        draw: data.draw,
                        recordsTotal: 0,
                        recordsFiltered: 0,
                        data: []
                    });
                });
        },
        columns: [
            { data: "title" },
            { data: "description" },
            {
                data: "id",
                orderable: false,
                searchable: false,
                render: function (data) {
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

    $('#projectsViewTable').on('click', '.delete-project', async function (e) {
        e.preventDefault();
        const id = $(this).data('id');
        const response = await fetch(`/api/project/${id}`, {
            method: 'DELETE'
        });
        //ADD MODAL
        table.ajax.reload(null, false);
    });

    $('#projectsViewTable tbody').on('click', 'tr', function (e) {
        if ($(e.target).closest('.dropdown, .dropdown-menu').length === 0) {
            window.location.href = '/projects/valuations';
        }
    });

    document.getElementById('addProjectBtn').onclick = function () {
        window.location.href = '/projects/add';
    };
};
