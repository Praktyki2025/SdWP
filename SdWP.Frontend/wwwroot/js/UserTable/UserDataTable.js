window.userDataTable = {
    init: function (tableSelector) {
        if ($.fn.dataTable.isDataTable(tableSelector)) $(tableSelector).DataTable().destroy();

        return $(tableSelector).DataTable({
            responsive: true,
            processing: true,
            serverSide: true,
            ajax: {
                url: '/api/admin/user/list',
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                },
                dataSrc: function (json) {
                    return json.data || [];
                }
            },
            columns: [
                { data: "name" },
                { data: "email" },
                {
                    data: "roles",
                    render: function (data) {
                        return Array.isArray(data) ? data.join(', ') : 'No roles';
                    }
                },
                {
                    data: "createdAt",
                    render: function (data) {
                        if (!data) return '';

                        try {
                            const date = new Date(data);
                            if (isNaN(date.getTime())) return data;


                            const day = String(date.getDate()).padStart(2, '0');
                            const month = String(date.getMonth() + 1).padStart(2, '0');
                            const year = date.getFullYear();
                            const hours = String(date.getHours()).padStart(2, '0');
                            const minutes = String(date.getMinutes()).padStart(2, '0');

                            return `${day}.${month}.${year} ${hours}:${minutes}`;
                        } catch (error) {
                            return data;
                        }
                    }
                },
                {
                    data: "isLocked",
                    title: "Status",
                    render: function (data) {
                        return data ? '<span class="badge bg-danger">Locked</span>' : '<span class="badge bg-success">Active</span>';
                    }
                },
                {
                    data: "id",
                    orderable: false,
                    searchable: false,
                    render: function (id) {
                        return `
                            <div class="dropdown">
                                <button class="btn btn-sm btn-outline-secondary dropdown-toggle" type="button" data-bs-toggle="dropdown">
                                    <i class="fas fa-cog"></i> Actions
                                </button>
                                <ul class="dropdown-menu">
                                    <li><a class="dropdown-item edit-user" data-id="${id}"><i class="fas fa-edit me-2"></i>Edit</a></li>
                                    <li><a class="dropdown-item text-danger delete-user" data-id="${id}"><i class="fas fa-trash-alt me-2"></i>Delete</a></li>
                                </ul>
                            </div>
                        `;
                    }
                }
            ]
        });
    }
}