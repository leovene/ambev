import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SalesService } from '../../../../services/sales.service';

@Component({
  selector: 'app-sales-form',
  templateUrl: './sales-form.component.html',
  styleUrls: ['./sales-form.component.scss'],
  standalone: false
})
export class SalesFormComponent implements OnInit {
  saleForm!: FormGroup;
  isEdit = false;

  constructor(
    private fb: FormBuilder,
    private salesService: SalesService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.saleForm = this.fb.group({
      id: [''],
      customerId: ['', Validators.required],
      branch: ['', Validators.required],
      items: this.fb.array([]), 
    });

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit = true;
      this.salesService.getSaleById(id).subscribe((sale) => {
        this.saleForm.patchValue({
          id: sale.id,
          customerId: sale.customerId,
          branch: sale.branch,
        });

        sale.items.forEach((item) => this.addItem(item));
      });
    }
  }

  get items(): FormArray {
    return this.saleForm.get('items') as FormArray;
  }

  addItem(item: any = { productId: '', quantity: 0, unitPrice: 0 }): void {
    this.items.push(
      this.fb.group({
        productId: [item.productId, Validators.required],
        quantity: [item.quantity, [Validators.required, Validators.min(1)]],
        unitPrice: [item.unitPrice, [Validators.required, Validators.min(0)]],
      })
    );
  }

  removeItem(index: number): void {
    this.items.removeAt(index);
  }

  onSubmit(): void {
    if (this.saleForm.valid) {
      const sale = this.saleForm.value;
      if (this.isEdit) {
        this.salesService.updateSale(sale.id, sale).subscribe(() => this.router.navigate(['/sales/sales-list']));
      } else {
        this.salesService.createSale(sale).subscribe(() => this.router.navigate(['/sales/sales-list']));
      }
    }
  }

  cancel(): void {
    this.router.navigate(['/sales/sales-list']);
  }
}