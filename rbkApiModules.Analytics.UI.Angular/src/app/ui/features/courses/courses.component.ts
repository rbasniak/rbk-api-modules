import { Component, OnInit } from '@angular/core';
import { environment } from '@environments/environment';
import { CoursesDetails } from '@models/courses-details';
import { UntilDestroy } from '@ngneat/until-destroy';
import { Select, Store } from '@ngxs/store';
import { CoursesActions } from '@state/database/courses/courses.actions';
import { CoursesSelectors } from '@state/database/courses/courses.selectors';
import { showConfirmation, showDialog } from 'ngx-rbk-utils';
import { SmzContentType, SmzFilterType, SmzTableConfig } from 'ngx-smz-ui';
import { Observable } from 'rxjs';

@UntilDestroy()
@Component({
  selector: 'app-courses',
  templateUrl: 'courses.component.html',
  styleUrls: ['courses.component.scss']
})
export class CoursesComponent implements OnInit {
  @Select(CoursesSelectors.all) public courses$: Observable<CoursesDetails[]>;
  public config: SmzTableConfig;
  public serverUrl = environment.serverUrl;

  constructor(private store: Store) {
    this.config = {
      showActions: true,
      showCaption: true,
      menu: [
        { label: 'Editar', icon: 'pi pi-fw pi-plus', command: (event: CoursesDetails): void => this.onUpdate(event) },
        { separator: true },
        { label: 'Apagar', icon: 'pi pi-fw pi-trash', command: (event: CoursesDetails): void => this.onDelete(event.id) },
      ],
      columns: [
        {
          contentType: SmzContentType.TEXT,
          contentData: { useTemplate: true },
          field: 'name',
          filterType: SmzFilterType.TEXT,
          header: 'Nome',
          isGlobalFilterable: true,
          isOrderable: false,
          showFilter: true,
          isVisible: true,
          width: '25em'
        },
        {
          contentType: SmzContentType.TEXT,
          contentData: { useTemplate: false },
          field: 'description',
          filterType: SmzFilterType.TEXT,
          header: 'Descrição',
          isGlobalFilterable: true,
          isOrderable: false,
          showFilter: true,
          isVisible: true,
        },
      ]
    };

  }
  public ngOnInit(): void {
  }

  public onCreate(): void {
    showDialog<CoursesDetails>(null, 'course', 'Criação de curso',
      (data) => this.store.dispatch(new CoursesActions.Create(data)));
  }

  public onUpdate(course: CoursesDetails): void {
    showDialog<CoursesDetails>(course, 'course', 'Edição de instituição',
      (data) => this.store.dispatch(new CoursesActions.Update(data)));
  }

  public onDelete(id: string): void {
    showConfirmation('Confirmação', 'Tem certeza que deseja excluir o curso e todo seu conteúdo?',
      () => this.store.dispatch(new CoursesActions.Delete(id)));
  }
}