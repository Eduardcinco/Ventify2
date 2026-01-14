import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ProductsService, Product } from '../../services/products.service';
import { VentasService } from '../../services/ventas.service';

@Component({
  standalone: true,
  selector: 'app-dashboard',
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  userName = '';
  products: Product[] = [];
  loadingProducts = false;
  productForm = {
    nombre: '',
    descripcion: '',
    precio: 0,
    costo: 0,
    cantidad: 1,
    stockMinimo: 1
  };

  sale = {
    productId: 0,
    cantidad: 1,
    paymentMethod: 'efectivo'
  };
  saleTotal = 0;
  saleMessage = '';
  saleError = '';

  constructor(
    private auth: AuthService,
    private productsService: ProductsService,
    private ventasService: VentasService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const session = this.auth.getSessionSync();
    this.userName = session?.nombre || session?.name || 'Usuario';
    this.loadProducts();
  }

  logout() {
    this.auth.logout().subscribe(() => this.router.navigate(['/login']));
  }

  loadProducts() {
    this.loadingProducts = true;
    this.productsService.getAll().subscribe({
      next: (list) => {
        this.products = list;
        this.loadingProducts = false;
        this.updateSaleTotal();
      },
      error: () => {
        this.loadingProducts = false;
      }
    });
  }

  createProduct() {
    const { nombre, descripcion, precio, costo, cantidad, stockMinimo } = this.productForm;
    if (!nombre || precio <= 0 || cantidad <= 0) return;
    this.productsService.create({
      nombre,
      descripcion,
      precioVenta: precio,
      precioCompra: costo || 0,
      cantidadInicial: cantidad,
      stockMinimo: stockMinimo || 1
    }).subscribe({
      next: () => {
        this.productForm = { nombre: '', descripcion: '', precio: 0, costo: 0, cantidad: 1, stockMinimo: 1 };
        this.loadProducts();
      }
    });
  }

  updateSaleTotal() {
    const product = this.products.find(p => p.id === Number(this.sale.productId));
    if (product) {
      this.saleTotal = product.precioVenta * (this.sale.cantidad || 0);
    } else {
      this.saleTotal = 0;
    }
  }

  registerSale() {
    this.saleMessage = '';
    this.saleError = '';
    const product = this.products.find(p => p.id === Number(this.sale.productId));
    if (!product) {
      this.saleError = 'Selecciona un producto';
      return;
    }
    if (this.sale.cantidad <= 0) {
      this.saleError = 'Cantidad inválida';
      return;
    }
    this.ventasService.create({
      items: [{ productoId: product.id, cantidad: this.sale.cantidad, precio: product.precioVenta }],
      total: product.precioVenta * this.sale.cantidad,
      paymentMethod: this.sale.paymentMethod,
      montoRecibido: this.sale.paymentMethod === 'efectivo' ? product.precioVenta * this.sale.cantidad : undefined,
      cambio: 0
    }).subscribe({
      next: () => {
        this.saleMessage = 'Venta registrada';
        this.loadProducts();
      },
      error: (err) => {
        this.saleError = err?.error?.message || 'No se pudo registrar la venta';
      }
    });
  }
}
