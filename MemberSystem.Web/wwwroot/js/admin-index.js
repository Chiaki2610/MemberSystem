document.getElementById('filterStatus').addEventListener('change', function () {
    const filterValue = this.value;
    const rows = document.querySelectorAll('tbody tr');

    rows.forEach(row => {
        const status = row.getAttribute('data-status');

        if (filterValue === 'all') {
            row.style.display = '';
        } else if (filterValue === 'approved' && status === 'approved') {
            row.style.display = '';
        } else if (filterValue === 'rejected' && status === 'rejected') {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
});

document.getElementById('resetFilter').addEventListener('click', function () {
    document.getElementById('filterStatus').value = 'all';
    document.querySelectorAll('tbody tr').forEach(row => row.style.display = '');
});

const memberModal = document.getElementById('memberModal');
memberModal.addEventListener('show.bs.modal', function (event) {

    const button = event.relatedTarget;


    const username = button.getAttribute('data-username');
    const fullname = button.getAttribute('data-fullname');
    const email = button.getAttribute('data-email');
    const dateOfBirth = new Date(button.getAttribute('data-dateOfBirth')).toLocaleDateString('zh-TW');
    const phone = button.getAttribute('data-phone');
    const bloodtype = button.getAttribute('data-bloodtype');

    document.getElementById('modal-username').textContent = username;
    document.getElementById('modal-fullname').textContent = fullname;
    document.getElementById('modal-email').textContent = email;
    document.getElementById('modal-dateOfBirth').textContent = dateOfBirth;
    document.getElementById('modal-phone').textContent = phone;
    document.getElementById('modal-bloodtype').textContent = bloodtype;
});