function initBulkDelete() {
    const selectAll = document.getElementById('selectAll');
    if (selectAll) {
        const checkboxes = document.querySelectorAll('.row-checkbox');
        const bulkDeleteBtn = document.getElementById('bulkDeleteBtn');
        const bulkDeleteIds = document.getElementById('bulkDeleteIds');
        const selectedCount = document.getElementById('selectedCount');

        function updateBulkDelete() {
            const checked = document.querySelectorAll('.row-checkbox:checked');
            bulkDeleteIds.innerHTML = '';
            checked.forEach(cb => {
                const input = document.createElement('input');
                input.type = 'hidden';
                input.name = 'ids';
                input.value = cb.value;
                bulkDeleteIds.appendChild(input);
            });

            if (checked.length > 0) {
                bulkDeleteBtn.classList.remove('d-none');
                selectedCount.textContent = checked.length + ' item(s) selected';
            } else {
                bulkDeleteBtn.classList.add('d-none');
                selectedCount.textContent = '';
            }
        }

        selectAll.addEventListener('change', function () {
            checkboxes.forEach(cb => cb.checked = this.checked);
            updateBulkDelete();
        });

        checkboxes.forEach(cb => cb.addEventListener('change', updateBulkDelete));
    }
}
initBulkDelete();


//const selectAll = document.getElementById('selectAll');
//if (selectAll) {
//    const checkboxes = document.querySelectorAll('.row-checkbox');
//    const bulkDeleteBtn = document.getElementById('bulkDeleteBtn');
//    const bulkDeleteIds = document.getElementById('bulkDeleteIds');
//    const selectedCount = document.getElementById('selectedCount');

//    function updateBulkDelete() {
//        const checked = document.querySelectorAll('.row-checkbox:checked');
//        bulkDeleteIds.innerHTML = '';
//        checked.forEach(cb => {
//            const input = document.createElement('input');
//            input.type = 'hidden';
//            input.name = 'ids';
//            input.value = cb.value;
//            bulkDeleteIds.appendChild(input);
//        });

//        if (checked.length > 0) {
//            bulkDeleteBtn.classList.remove('d-none');
//            selectedCount.textContent = checked.length + ' item(s) selected';
//        } else {
//            bulkDeleteBtn.classList.add('d-none');
//            selectedCount.textContent = '';
//        }
//    }

//    selectAll.addEventListener('change', function () {
//        checkboxes.forEach(cb => cb.checked = this.checked);
//        updateBulkDelete();
//    });

//    checkboxes.forEach(cb => cb.addEventListener('change', updateBulkDelete));
//}