let selectedProjectId = null;

window.projectsEvents = {
    register: function (tableSelector, tableInstance) {
        $(tableSelector).on('click', '.edit-project', function (e) {
            e.preventDefault();
            const id = $(this).data('id');
            window.location.href = `/projects/edit?id=${id}`;
        });

        $(tableSelector).on('click', '.delete-project', function (e) {
            e.preventDefault();
            selectedProjectId = $(this).data('id');
            const myModal = new bootstrap.Modal(document.getElementById('DeleteModal')).show();
        });

        $('#confirmButton').on('click', async function () {
            if (selectedProjectId === null) return;

            try {
                await window.projectsApi.deleteProject(selectedProjectId);
                tableInstance.ajax.reload(null, false);
                showToast("Project deleted successfully", 'bg-success');
            } catch {
                showToast("Operation failed", "bg-danger");
            }

            const modalEl = document.getElementById('DeleteModal');
            const modalInstance = bootstrap.Modal.getInstance(modalEl);
            modalInstance.hide();

            selectedProjectId = null;
        });

        $(`${tableSelector} tbody`).on('click', 'tr', function (e) {
            e.preventDefault();
            if ($(this).find('td').hasClass('dataTables_empty')) {
                return;
            }
            const id = $(this).data('id');
            if ($(e.target).closest('.dropdown, .dropdown-menu').length === 0) {
                //window.location.href = `/projects/valuations?id=${id}`;
                window.location.href = `/projects/valuations`;
            }
        });

    }
}

//bgclass - bg-success or bg-danger example: showToast("Project deleted successfully", 'bg-success');
function showToast(message, bgClass) {
    const toastElement = document.getElementById('toast');
    const toastBody = toastElement.querySelector('.toast-body');
    toastBody.textContent = message;

    toastElement.className = toastElement.className
        .split(' ')
        .filter(cls => !cls.startsWith('bg-'))
        .join(' ');

    toastElement.classList.add(bgClass);

    const toast = new bootstrap.Toast(toastElement);
    toast.show();
}




