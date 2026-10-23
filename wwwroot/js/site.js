document.addEventListener('DOMContentLoaded', () => {
    const reportDialog = document.getElementById('reportDialog');
    const reportContent = document.getElementById('reportDialogContent');

    // -----------------------------------------------------------------------------------------------------------
    //                                                                                          REPORT FUNCTIONS
    // -----------------------------------------------------------------------------------------------------------

    if (reportDialog && reportContent) {
        document.querySelectorAll('.report-row').forEach(row => {
            row.addEventListener('click', async () => {
                const reportId = row.dataset.reportId;
                const response = await fetch(`?handler=ReportDetails&id=${reportId}`);

                if (!response.ok) {
                    alert('Unable to load report details.');
                    return;
                }

                reportContent.innerHTML = await response.text();
                reportDialog.showModal();
            });
        });

        reportDialog.querySelector('[data-close-report]')?.addEventListener('click', () => {
            reportDialog.close();
        });
    }

    const searchInput = document.getElementById('reportSearch');

    if (searchInput) {
        const reportRows = document.querySelectorAll('.report-row');

        searchInput.addEventListener('input', () => {
            const searchTerm = searchInput.value.toLowerCase().trim();

            reportRows.forEach(row => {
                const reportText = row.textContent.toLowerCase();
                const matches = reportText.includes(searchTerm);

                row.style.display = matches ? '' : 'none';
            });
        });
    }

    if (!document.getElementById('unitDialog') || !document.getElementById('weaponDialog')) {
        return;
    }

    // -----------------------------------------------------------------------------------------------------------
    //                                                                                       SELECT CLICKED ROWS
    // -----------------------------------------------------------------------------------------------------------

    let selectedUnit = null;
    let selectedWeapon = null;

    // Select a unit by clicking its row.
    document.querySelectorAll('#unitsTable tbody tr[data-id]').forEach(row => {
        row.addEventListener('click', () => {
            document.querySelectorAll('#unitsTable tbody tr[data-id]')
                .forEach(otherRow => otherRow.classList.remove('is-selected'));

            row.classList.add('is-selected');
            selectedUnit = row.dataset;
        });
    });

    // Select a weapon by clicking its row.
    document.querySelectorAll('#weaponsTable tbody tr[data-id]').forEach(row => {
        row.addEventListener('click', () => {
            document.querySelectorAll('#weaponsTable tbody tr[data-id]')
                .forEach(otherRow => otherRow.classList.remove('is-selected'));

            row.classList.add('is-selected');
            selectedWeapon = row.dataset;
        });
    });

    // -----------------------------------------------------------------------------------------------------------
    //                                                                                                UNIT LOGIC
    // -----------------------------------------------------------------------------------------------------------

    // Get references to the unit buttons.
    const addButton = document.getElementById('addUnitButton');
    const deleteButton = document.getElementById('deleteUnitButton');
    const editButton = document.getElementById('updateUnitButton');

    // Get references to the unit dialog and form.
    const dialog = document.getElementById('unitDialog');
    const form = dialog.querySelector('form');
    const addSubmit = document.getElementById('addUnitSubmit');
    const editSubmit = document.getElementById('editUnitSubmit');
    const deleteForm = document.getElementById('deleteUnitForm');

    if (dialog.dataset.reopen === 'true') {
        dialog.showModal();
    }
    
    //===============================
    // Add Unit: open an empty form.
    //===============================
    addButton.addEventListener('click', () => {
        form.reset();
        form.elements['SelectedUnitId'].value = '0';
        form.elements['UnitInput.Name'].value = '';
        form.elements['UnitInput.Type'].value = '';
        form.elements['UnitInput.Health'].value = '1';
        form.elements['UnitInput.WeaponId'].value = '0';
        form.elements['UnitInput.Armor'].value = '0';
        form.elements['UnitInput.AttackSkill'].value = '0';
        form.elements['UnitInput.DodgeChance'].value = '0';
        form.elements['UnitInput.BlockChance'].value = '0';
        form.elements['UnitInput.AbilityIds'].value = '';

        addSubmit.hidden = false;
        editSubmit.hidden = true;

        dialog.showModal();
    });

    //=================================================
    // Edit Unit: fill the form with the selected row.
    //=================================================
    editButton.addEventListener('click', () => {
        if (!selectedUnit) {
            alert('Please select a unit first.');
            return;
        }

        form.elements['SelectedUnitId'].value = selectedUnit.id;
        form.elements['UnitInput.Name'].value = selectedUnit.name;
        form.elements['UnitInput.Type'].value = selectedUnit.type;
        form.elements['UnitInput.Health'].value = selectedUnit.health;
        form.elements['UnitInput.WeaponId'].value = selectedUnit.weaponId;
        form.elements['UnitInput.Armor'].value = selectedUnit.armor;
        form.elements['UnitInput.AttackSkill'].value = selectedUnit.attackSkill;
        form.elements['UnitInput.DodgeChance'].value = selectedUnit.dodgeChance;
        form.elements['UnitInput.BlockChance'].value = selectedUnit.blockChance;
        form.elements['UnitInput.AbilityIds'].value = selectedUnit.abilityIds;

        addSubmit.hidden = true;
        editSubmit.hidden = false;

        dialog.showModal();
    });

    //========================================================
    // Delete Unit: deletes selected unit after confirmation.
    //========================================================
    deleteButton.addEventListener('click', () => {
        if (!selectedUnit) {
            alert('Please select a unit first.');
            return;
        }

        if (confirm(`Are you sure you want to delete the unit "${selectedUnit.name}"?`)) {
            deleteForm.elements['SelectedUnitId'].value = selectedUnit.id;
            deleteForm.requestSubmit();
        }
    });

    // =================================================================
    // Close Dialog: close the dialog when the close button is clicked.
    // =================================================================
    dialog.querySelector('[data-close-dialog]').addEventListener('click', () => {
        dialog.close();
    });

    // -----------------------------------------------------------------------------------------------------------
    //                                                                                              WEAPON LOGIC
    // -----------------------------------------------------------------------------------------------------------

    // Get references to the weapon buttons.
    const addWeaponButton = document.getElementById('addWeaponButton');
    const editWeaponButton = document.getElementById('updateWeaponButton');
    const deleteWeaponButton = document.getElementById('deleteWeaponButton');

    // Get references to the weapon dialog and form.
    const weaponDialog = document.getElementById('weaponDialog');
    const weaponForm = weaponDialog.querySelector('form');
    const addWeaponSubmit = document.getElementById('addWeaponSubmit');
    const editWeaponSubmit = document.getElementById('editWeaponSubmit');
    const deleteWeaponForm = document.getElementById('deleteWeaponForm');

    if (weaponDialog.dataset.reopen === 'true') {
        weaponDialog.showModal();
    }

    //=================================
    // Add Weapon: open an empty form.
    //=================================
    addWeaponButton.addEventListener('click', () => {
        weaponForm.reset();
        weaponForm.elements['SelectedWeaponId'].value = '0';
        weaponForm.elements['WeaponInput.Name'].value = '';
        weaponForm.elements['SelectedWeapon'].value = '';
        weaponForm.elements['WeaponInput.Damage'].value = '1';
        weaponForm.elements['WeaponInput.NumberOfAttacks'].value = '1';
        weaponForm.elements['WeaponInput.AttackModifier'].value = '0';
        weaponForm.elements['WeaponInput.HitModifier'].value = '0';

        addWeaponSubmit.hidden = false;
        editWeaponSubmit.hidden = true;

        weaponDialog.showModal();
    });

    //===================================================
    // Edit Weapon: fill the form with the selected row.
    //===================================================
    editWeaponButton.addEventListener('click', () => {
        if (!selectedWeapon) {
            alert('Please select a weapon first.');
            return;
        }

        weaponForm.elements['SelectedWeaponId'].value = selectedWeapon.id;
        weaponForm.elements['WeaponInput.Name'].value = selectedWeapon.name;
        weaponForm.elements['SelectedWeapon'].value = selectedWeapon.type;
        weaponForm.elements['WeaponInput.Damage'].value = selectedWeapon.damage;
        weaponForm.elements['WeaponInput.NumberOfAttacks'].value = selectedWeapon.numberOfAttacks;
        weaponForm.elements['WeaponInput.AttackModifier'].value = selectedWeapon.attackModifier;
        weaponForm.elements['WeaponInput.HitModifier'].value = selectedWeapon.hitModifier;

        addWeaponSubmit.hidden = true;
        editWeaponSubmit.hidden = false;

        weaponDialog.showModal();
    });

    //============================================================
    // Delete Weapon: deletes selected weapon after confirmation.
    //============================================================
    deleteWeaponButton.addEventListener('click', () => {
        if (!selectedWeapon) {
            alert('Please select a weapon first.');
            return;
        }

        if (confirm(`Are you sure you want to delete the weapon "${selectedWeapon.name}"?`)) {
            deleteWeaponForm.elements['SelectedWeaponId'].value = selectedWeapon.id;
            deleteWeaponForm.requestSubmit();
        }
    });

    //=========================================================================
    // Close Weapon Dialog: close the dialog when the close button is clicked.
    //=========================================================================
    weaponDialog.querySelector('[data-close-dialog]').addEventListener('click', () => {
        weaponDialog.close();
    });
});